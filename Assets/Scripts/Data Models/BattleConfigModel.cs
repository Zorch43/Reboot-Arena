using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class BattleConfigModel
    {
        public List<PlayerConfigModel> Players { get; set; }

        public bool IsPlayerSpectator
        {
            get
            {
                foreach(var p in Players)
                {
                    if(p.Controller == PlayerConfigModel.ControlType.Player)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool IsValidConfig
        {
            get
            {
                //rule: must be at least two teams in the battle
                if(Players.Count < 2)
                {
                    return false;
                }
                //rule: up to one player is allowed in the battle
                int playerCount = 0;
                foreach (var p in Players)
                {
                    if (p.Controller == PlayerConfigModel.ControlType.Player)
                    {
                        playerCount++;
                    }
                }
                if(playerCount > 1)
                {
                    return false;
                }
                //TODO: other conditions
                return true;
            }
        }

        public PlayerConfigModel PlayerTeam
        {
            get
            {
                foreach(var p in Players)
                {
                    if(p.Controller == PlayerConfigModel.ControlType.Player)
                    {
                        return p;
                    }
                }
                return null;
            }
        }
        public bool IsAITeam(int teamId)
        {
            foreach(var p in Players)
            {
                if(p.TeamId == teamId && p.Controller == PlayerConfigModel.ControlType.AI)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
