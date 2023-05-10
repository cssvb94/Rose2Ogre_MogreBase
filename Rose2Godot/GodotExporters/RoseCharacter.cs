using Revise.CHR;
using System;

namespace Rose2Godot.GodotExporters
{
    public class RoseCharacter
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetLogger("RoseCharacter");
        private const string NPC_CHARS_PATH = "3DDATA/LIST_NPC.CHR";
        public RoseCharacter(string GodotProjectPah) {

            CharacterFile npc = new CharacterFile();
            try
            {
                npc.Load(NPC_CHARS_PATH);
            }
            catch (Exception x)
            {
                log.Fatal(x);
                throw;
            }
            


        }
    }
}
