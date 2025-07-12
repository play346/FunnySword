using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace WrathofWaffle.Content.NPCs
{
    public class Fabsol : ModNPC
    {
        private double frameCounter = 23;
        
        private Vector2 phase3TargetPos;
        private int phase3MoveTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 23;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 300;
            NPC.width = 48;
            NPC.height = 42;
            NPC.defense = 20;
            NPC.lifeMax = 50000;
            NPC.knockBackResist = 0.4f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.DD2_OgreHurt;
            NPC.DeathSound = SoundID.DD2_OgreDeath;
            NPC.rarity = 1;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            if (NPC.velocity.Y != 0f && !NPC.IsABestiaryIconDummy)
            {

                NPC.frame.Y = frameHeight * 5;
                return;
            }

            NPC.frameCounter += NPC.IsABestiaryIconDummy ? 0.5 : NPC.velocity.Length() * 0.2;

            if (NPC.frameCounter >= 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.MasterBait, chanceDenominator: 10));
            npcLoot.Add(ItemDropRule.Common(ItemID.HairDyeRemover, chanceDenominator: 22));
            npcLoot.Add(ItemDropRule.Common(ItemID.OldMiner, chanceDenominator: 25));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                }
            }
        }

        public override void OnKill()
        {

        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {

            if (spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !spawnInfo.PlayerInTown && Main.invasionType <= 0)
            {
                
                return 0.4f;
            }
            return 0f;
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            NPC.TargetClosest();

            float distanceToPlayer = Vector2.Distance(NPC.Center, target.Center);

            
            if (NPC.life < NPC.lifeMax / 2 && NPC.ai[1] == 0 && NPC.ai[2] == 0)
            {
                NPC.ai[2] = 180; 
                NPC.velocity = Vector2.Zero;
                NPC.dontTakeDamage = true; 
                for (int i = 0; i < 30; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldFlame);
                Main.NewText("Wait, I'm sending you an invite...",200, 60);
                return;
            }

            
            if (NPC.ai[2] > 0)
            {
                NPC.velocity = Vector2.Zero;
                NPC.dontTakeDamage = true;
                NPC.ai[2]--;
                if (NPC.ai[2] == 0)
                {
                    NPC.ai[1] = 1; 
                    NPC.dontTakeDamage = false;
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center); 
                    Main.NewText("Fabsol has invited you to his Discord server!", 255, 60, 60);
                }
                return;
            }

            
            NPC.dontTakeDamage = false;

            if (NPC.ai[1] == 1)
            {
                
                Vector2 targetPosition = target.Center + new Vector2(0, -500);
                NPC.Center = targetPosition;
                NPC.velocity = Vector2.Zero;

                
                NPC.ai[0]++;
                if (NPC.ai[0] >= 70)
                {
                    NPC.ai[0] = 0;

                    if (distanceToPlayer < 1500f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SoundEngine.PlaySound(SoundID.Item43, NPC.Center);

                        int numberProjectiles = 9;

                        float spread = MathHelper.ToRadians(Main.rand.NextFloat(100f, 400f));
                        float speed = 25f;
                        int type = ProjectileID.HallowBossLastingRainbow;
                        int damage = 50;

                        Vector2 shootDirection = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            float rotation = MathHelper.Lerp(-spread / 2, spread / 2, i / (numberProjectiles - 1f));
                            Vector2 perturbedDirection = shootDirection.RotatedBy(rotation) * speed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedDirection, type, damage, 1f, Main.myPlayer);
                        }
                    }
                }

                if (Main.rand.NextBool(5))
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.RainbowTorch, 0, 0);
                }
            }
            else
            {
               
                if (distanceToPlayer < 1200f && NPC.velocity.Y == 0f)
                {
                    NPC.velocity.Y = -8f;
                    NPC.velocity.X = (target.Center.X > NPC.Center.X ? 1 : -1) * 4f;
                }

                NPC.ai[0]++;
                if (NPC.ai[0] >= 50)
                {
                    NPC.ai[0] = 0;

                    if (distanceToPlayer < 1200f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

                        int numberProjectiles = 5;
                        float spread = MathHelper.ToRadians(40);

                        float speed = 20f;
                        int type = ProjectileID.CursedFlameHostile;
                        int damage = 60;

                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            float rotation = MathHelper.Lerp(-spread / 2, spread / 2, i / (numberProjectiles - 1f));
                            Vector2 perturbedDirection = direction.RotatedBy(rotation) * speed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedDirection, type, damage, 1f, Main.myPlayer);
                        }
                    }
                }

                if (Main.rand.NextBool(10))
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldFlame, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                }
            }

           
            if (NPC.life < NPC.lifeMax * 0.2 && NPC.ai[3] == 0 && NPC.ai[1] == 1)
            {
                NPC.ai[3] = 1; 
                for (int i = 0; i < 40; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.RainbowTorch);
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                Main.NewText("Fabsol is going berserk!", 255, 0, 100);

                
                float offsetX = Main.rand.NextFloat(-500f, 500f);
                float offsetY = Main.rand.NextFloat(-700f, -200f);
                phase3TargetPos = target.Center + new Vector2(offsetX, offsetY);
                phase3MoveTimer = 0;
            }

           
            if (NPC.ai[1] == 1 && NPC.ai[3] == 1)
            {
                phase3MoveTimer++;
                if (phase3MoveTimer >= 120) 
                {
                    phase3MoveTimer = 0;
                    float offsetX = Main.rand.NextFloat(-500f, 500f);
                    float offsetY = Main.rand.NextFloat(-700f, -200f);
                    phase3TargetPos = target.Center + new Vector2(offsetX, offsetY);

                   
                    for (int i = 0; i < 10; i++)
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TeleportationPotion);
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                }

                
                float moveSpeed = 18f;
                float inertia = 40f;
                Vector2 moveTo = phase3TargetPos - NPC.Center;
                if (moveTo.Length() > moveSpeed)
                {
                    moveTo.Normalize();
                    moveTo *= moveSpeed;
                }
                NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;

                
                NPC.ai[0]++;
                if (NPC.ai[0] >= 60)
                {
                    NPC.ai[0] = 0;

                    if (distanceToPlayer < 2000f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SoundEngine.PlaySound(SoundID.Item84, NPC.Center);

                        int numberProjectiles = 11;
                        float spread = MathHelper.ToRadians(Main.rand.NextFloat(120f, 250f));
                        float speed = 40f;
                        int type = ProjectileID.InsanityShadowHostile;
                        int damage = 100;

                        Vector2 shootDirection = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            float rotation = MathHelper.Lerp(-spread / 2, spread / 2, i / (numberProjectiles - 1f));
                            Vector2 perturbedDirection = shootDirection.RotatedBy(rotation) * speed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedDirection, type, damage, 1f, Main.myPlayer);
                        }
                    }
                }

                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.RainbowTorch, 0, 0);
                }
                return;
            }
        }
    }
}









