using System;
using System.Collections.Generic;
using Ecslent2D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Ecslent2D.Collision
{

    public interface IGridAssetCongolmerate : IGridAsset
    {
        List<IGridAsset> members { get; set; }
    }

    public interface IGridAsset : IDrawableAsset
    {
        List<Point> gridPositions { get; set; }
        bool dead { get; set; }

        CollisionHandler collision { get; set; }
        void ScheduleCollisionHandling(List<Type> conditions = null);
        void RemoveFromCollisionHandling();
        void HandleCollision(IGridAsset other, Point gPos);
        void Update(GameTime gameTime);
        void Spawn();
    }
}
