using Compendium.Values;

using System;

namespace Compendium.Players
{
    /// <summary>
    /// Represents the user ID of an in-game player.
    /// </summary>
    public class PlayerUserId : IGetValue<string>
    {
        /// <summary>
        /// Gets the player's full user ID.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the player's user ID type <see cref="String"/> (steam, discord, northwood).
        /// </summary>
        public string TypeValue { get; }

        /// <summary>
        /// Gets the player's ID as a clean <see cref="String"/>.
        /// </summary>
        public string Clean { get; }

        /// <summary>
        /// Gets the player's clean user ID, parsed to a <see cref="Int64"/>.
        /// </summary>
        public long ParsedId { get; }

        /// <summary>
        /// Gets the player's user ID type.
        /// </summary>
        public PlayerUserIdType Type { get; }

        /// <summary>
        /// Initializes a new <see cref="PlayerUserId"/> instance.
        /// </summary>
        /// <param name="value">The user ID string to parse.</param>
        public PlayerUserId(string value)
        {

        }
    }
}