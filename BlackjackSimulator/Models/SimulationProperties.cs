using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackSimulator.Entities.Interfaces;

namespace BlackjackSimulator.Models
{
    public class SimulationProperties
    {
        public decimal MinimumBetForTable { get; set; }
        public decimal MaximumBetForTable { get; set; }
        public int MaximumPlayersForTable { get; set; }
        public int NumberOfDecksInShoe { get; set; }
        public List<PlayerProperties> PlayerPropertiesCollection { get; set; }
    }
}
