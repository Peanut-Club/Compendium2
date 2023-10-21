using Mono.Cecil.Cil;
using Mono.Cecil;

using MonoMod.Utils;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public delegate object FastDelegate(object target, object[] args);

    public static class FastReflection
    {
        private static readonly Dictionary<MethodBase, FastDelegate> _delegateCache = new Dictionary<MethodBase, FastDelegate>();

        private static readonly Type[] _fastInvokerArgs = new Type[] { typeof(object), typeof(object[]) };

        public static MethodBase GetRepresentedMethod(this MethodBase delegateMethod)
        {
            foreach (var pair in _delegateCache)
            {
                if (pair.Value.Method == delegateMethod)
                    return pair.Key;
            }

            return delegateMethod;
        }

        public static MethodBase GetUnderlyingMethod(this FastDelegate fastDelegate)
        {
            foreach (var pair in _delegateCache)
            {
                if (pair.Value == fastDelegate)
                    return pair.Key;
            }

            return null;
        }

        public static FastDelegate GetFastInvoker(this MethodBase method, bool allowDirectValueAccess = true)
        {
            if (_delegateCache.TryGetValue(method, out var del))
                return del;

            var dynamicMethod = new DynamicMethodDefinition($"FastDelegate<{method.GetID(simple: true)}>", typeof(object), _fastInvokerArgs);
            var il = dynamicMethod.GetILProcessor();
            var args = method.GetParameters();
            var genLocalBoxPtr = true;

            if (!method.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);

                if (method.DeclaringType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, method.DeclaringType);
            }

            for (int i = 0; i < args.Length; i++)
            {
                var type = args[i].ParameterType;
                var isRef = type.IsByRef;

                if (isRef)
                    type = type.GetElementType();

                var isValueType = type.IsValueType;

                if (isRef && isValueType && !allowDirectValueAccess)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4, i);
                }

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4, i);

                if (isRef && !isValueType)
                    il.Emit(OpCodes.Ldelema, typeof(object));
                else
                {
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (isValueType)
                    {
                        if (!isRef || !allowDirectValueAccess)
                        {
                            il.Emit(OpCodes.Unbox_Any, type);

                            if (isRef)
                            {
                                il.Emit(OpCodes.Box, type);
                                il.Emit(OpCodes.Dup);
                                il.Emit(OpCodes.Unbox, type);

                                if (genLocalBoxPtr)
                                {
                                    genLocalBoxPtr = false;
                                    dynamicMethod.Definition.Body.Variables.Add(new VariableDefinition(new PinnedType(dynamicMethod.Definition.Module.TypeSystem.Void)));
                                }

                                il.Emit(OpCodes.Stloc_0);
                                il.Emit(OpCodes.Stelem_Ref);
                                il.Emit(OpCodes.Ldloc_0);
                            }
                        }
                        else
                            il.Emit(OpCodes.Unbox, type);
                    }
                }
            }

            if (method.IsConstructor)
                il.Emit(OpCodes.Newobj, (ConstructorInfo)method);
            else if (method.IsFinal || !method.IsVirtual)
                il.Emit(OpCodes.Call, (MethodInfo)method);
            else
                il.Emit(OpCodes.Callvirt, (MethodInfo)method);

            var returnType = method.IsConstructor ? method.DeclaringType : ((MethodInfo)method).ReturnType;

            if (returnType != typeof(void))
            {
                if (returnType.IsValueType)
                    il.Emit(OpCodes.Box, returnType);
            }
            else
                il.Emit(OpCodes.Ldnull);

            il.Emit(OpCodes.Ret);

            var dynamicMethodValue = dynamicMethod.Generate();
            var fastDelegate = (FastDelegate)dynamicMethodValue.CreateDelegate(typeof(FastDelegate));

            _delegateCache[method] = fastDelegate;

            return fastDelegate;
        }
    }
}