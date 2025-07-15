using UnityEngine;

namespace GimmeDOTSGeometry.Samples.ECS
{
    public class QuadtreeMovementECSSystemGUI : SystemGUI
    {

        public QuadtreeMovementECSSystem system;

        private void Start()
        {
            
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            var areaRect = new Rect(0, 0, 350, 400);
            GUI.Box(areaRect, string.Empty);
            GUILayout.BeginArea(areaRect);
            GUI.DrawTexture(areaRect, this.background, ScaleMode.StretchToFill);
            GUILayout.Box("ECS Quadtree Movement GUI");
            GUILayout.Label($"Current points: {this.system.GetNrOfPoints()}", this.textStyle);

            GUILayout.Label($"Quadtree Depth {this.system.CurrentSearchDepth}");

            int depth = (int)GUILayout.HorizontalSlider(this.system.CurrentSearchDepth, 1, 8);
            if(depth != this.system.CurrentSearchDepth)
            {
                this.system.CurrentSearchDepth = depth;
            }

            GUILayout.Label($"Search Radius Percentage: {this.system.CurrentSearchPercentage}");
            float percentage = GUILayout.HorizontalSlider(this.system.CurrentSearchPercentage, 0.0f, 1.0f);
            if(percentage != this.system.CurrentSearchPercentage)
            {
                this.system.CurrentSearchPercentage = percentage;
            }

            if(!this.system.IsDoingMultiQueries())
            {
                if(this.GUIButton("Do Multi-Query?"))
                {
                    this.system.EnableMultiQuery(true);
                }

            } else
            {
                if(this.GUIButton("Do Mono-Query?"))
                {
                    this.system.EnableMultiQuery(false);
                }
            }

            GUILayout.Label($"Percentage of Moving Objects: {this.system.CurrentMovingPercentage}");
            float movingPercentage = GUILayout.HorizontalSlider(this.system.CurrentMovingPercentage, 0.0f, 1.0f);
            if(movingPercentage != this.system.CurrentMovingPercentage)
            {
                this.system.CurrentMovingPercentage = movingPercentage;
            }

            if(this.GUIButton("Add 1000 Points"))
            {
                this.system.AddPoints(1000);
            }

            if(this.system.IsUsingSparseQuadtree())
            {

                if(this.GUIButton("Use Dense Quadtree"))
                {
                    this.system.ExchangeQuadtreeModel();
                }

            } else
            {
                if(this.GUIButton("Use Sparse Quadtree"))
                {
                    this.system.ExchangeQuadtreeModel();
                }
            }

            GUILayout.EndArea();
        }

    }
}
