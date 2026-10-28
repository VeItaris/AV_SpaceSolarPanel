using RimWorld;
using UnityEngine;
using Verse;

namespace AV_SpaceSolarPanel
{

    [StaticConstructorOnStartup]
    public class CompPowerPlantSolar_Space : CompPowerPlant
    {
        private const float NightPower = 0f;

        private static readonly Vector2 BarSize = new Vector2(0.7f, 0.12f);

        private static readonly Material PowerPlantSolarBarSpaceFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.65f, 0.475f, 0.1f));

        private static readonly Material PowerPlantSolarBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));

        private static readonly Material PowerPlantSolarBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.30f, 0.30f, 0.30f)); //new Color(0.15f, 0.15f, 0.15f));

        protected override float DesiredPowerOutput => Mathf.Lerp(NightPower, 0f - base.Props.PowerConsumption, parent.Map.skyManager.CurSkyGlow) * SpaceFactor * RoofedPowerOutputFactor;

        private float RoofedPowerOutputFactor
        {
            get
            {
                int tiles = 0;
                int roofed = 0;
                foreach (IntVec3 item in parent.OccupiedRect())
                {
                    tiles++;
                    if (parent.Map.roofGrid.Roofed(item))
                    {
                        roofed++;
                    }
                }
                return (float)(tiles - roofed) / (float)tiles;
            }
        }

        private float SpaceFactor
        {
            get
            {
                if (InSpace)
                {
                    return 1.3f;
                }

                return 1f;
            }
        }

        private bool InSpace = false;

        private int counter = 0;

        public override void CompTick()
        {
            base.CompTick();

            if (ModsConfig.OdysseyActive)
            {
                counter++;

                if(counter >= 600)
                {
                    if (parent?.Map?.Tile.LayerDef?.isSpace != null && parent.Map.Tile.LayerDef.isSpace)
                    {
                        InSpace = true;
                    }
                    else
                    {
                        InSpace = false;
                    }
                    counter = 0;
                }
                
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
            r.center = parent.DrawPos + Vector3.up * 0.1f;
            r.size = BarSize;
            r.fillPercent = base.PowerOutput / (0f - (base.Props.PowerConsumption * SpaceFactor));

            if(InSpace)
            {
                r.filledMat = PowerPlantSolarBarSpaceFilledMat;
            }
            else
            {
                r.filledMat = PowerPlantSolarBarFilledMat;
            }

            r.unfilledMat = PowerPlantSolarBarUnfilledMat;
            r.margin = 0.15f;
            Rot4 rotation = parent.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);
        }
    }

}
