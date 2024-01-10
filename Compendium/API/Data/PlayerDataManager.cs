using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compendium.API.Data
{
    public class PlayerDataManager
    {
        public PlayerDataManager(Player owner)
        {
            Owner = owner;
        }

        public Player Owner { get; }
    }
}