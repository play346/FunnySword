using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WrathofWaffle.Content.NPCs
{
    public class Fabsol : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 2;
            AIType = -1;
            NPC.damage = 100;
            NPC.width = 24;
            NPC.height = 42;
            NPC.defense = 20;
            NPC.lifeMax = 5000;
            NPC.knockBackResist = 0.4f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
        }
        public override void FindFrame(int frameHeight)
        {
            if ((NPC.velocity.Y > 0f || NPC.velocity.Y < 0f) && !NPC.IsABestiaryIconDummy)
            {
                NPC.spriteDirection = NPC.direction;
                NPC.frame.Y = frameHeight * 42;
                NPC.frameCounter = 42;
            }
            else
            {
                if (NPC.IsABestiaryIconDummy)
                {
                    NPC.frameCounter += 11;
                }
                else
                {
                    NPC.frameCounter += (double)(NPC.velocity.Length() / 100f);
                }
                NPC.spriteDirection = NPC.direction;
                if (NPC.frameCounter > 1)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 1;
                }
                if (NPC.frame.Y >= frameHeight * 1)
                {
                    NPC.frame.Y = 1;
                }
            }
        }
    }
}