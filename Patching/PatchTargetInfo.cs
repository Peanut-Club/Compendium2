using Compendium.Extensions;
using Compendium.Utilities.Reflection;

using System;
using System.Linq;
using System.Reflection;

namespace Compendium.Patching
{
    public struct PatchTargetInfo
    {
        private MethodInfo _resolveResult;

        public string TargetName;

        public PatchMemberType Type;

        public Type TargetType;

        public Type[] GenericArgs;
        public Type[] OverloadArgs;

        public PatchTargetInfo(Type type, string name, PatchMemberType patchMemberType)
        {
            TargetName = name;
            TargetType = type;
            Type = patchMemberType;
        }

        public PatchTargetInfo(Type type, string name, PatchMemberType patchMemberType, Type[] overloadArgs)
        {
            TargetName = name;
            TargetType = type;
            Type = patchMemberType;
            OverloadArgs = overloadArgs;
        }

        public PatchTargetInfo(Type type, string name, PatchMemberType patchMemberType, Type[] genericArgs, Type[] overloadArgs)
        {
            TargetName = name;
            TargetType = type;
            Type = patchMemberType;
            GenericArgs = genericArgs;
            OverloadArgs = overloadArgs;
        }

        public MethodInfo Resolve(bool includeCached)
        {
            if (_resolveResult != null && includeCached)
                return _resolveResult;

            if (TargetType is null)
                throw new ArgumentNullException(nameof(TargetType));

            if (string.IsNullOrWhiteSpace(TargetName))
                throw new ArgumentNullException(nameof(TargetName));

            switch (Type)
            {
                case PatchMemberType.Method:
                    {
                        var methods = TargetType.GetAllMethods();

                        for (int i = 0; i < methods.Length; i++)
                        {
                            if (methods[i].Name != TargetName)
                                continue;

                            if (GenericArgs != null && GenericArgs.Length > 0)
                            {
                                var genericArgs = methods[i].GetGenericArguments();

                                if (!genericArgs.IsMatch(GenericArgs))
                                    continue;
                            }

                            if (OverloadArgs != null && OverloadArgs.Length > 0)
                            {
                                var overloadArgs = methods[i].GetParameters().Select(p => p.ParameterType);

                                if (!overloadArgs.IsMatch(OverloadArgs))
                                    continue;
                            }

                            _resolveResult = methods[i];
                            break;
                        }

                        return _resolveResult;
                    }

                case PatchMemberType.Getter:
                    {
                        var properties = TargetType.GetAllProperties();

                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (properties[i].Name != TargetName)
                                continue;

                            var getter = properties[i].GetGetMethod(true);

                            if (getter is null)
                                continue;

                            _resolveResult = getter;
                            break;
                        }

                        return _resolveResult;
                    }

                case PatchMemberType.Setter:
                    {
                        var properties = TargetType.GetAllProperties();

                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (properties[i].Name != TargetName)
                                continue;

                            var setter = properties[i].GetSetMethod(true);

                            if (setter is null)
                                continue;

                            _resolveResult = setter;
                            break;
                        }

                        return _resolveResult;
                    }
            }

            return _resolveResult;
        }
    }
}