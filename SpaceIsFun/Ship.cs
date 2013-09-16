﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceIsFun
{
    class Grid : Entity
    {
        #region fields
        /// <summary>
        /// x,y position of the grid on the ship (ie, top left grid will be 1,1)
        /// </summary>
        private Vector2 gridPosition;

        public Vector2 GridPosition
        {
            get
            {
                return gridPosition;
            }

            set
            {
                gridPosition = value;
            }
        }

        /// <summary>
        /// drawable for the grid (this is probably never gonna move)
        /// </summary>
        private Drawable sprite;

        public Drawable Sprite
        {
            get
            {
                return sprite;
            }

            set
            {
                sprite = value;
            }
        }

        #endregion

        #region constructor / destructor
        public Grid()
            : base()
        {
        }

        public Grid(Texture2D spriteTexture, Vector2 position) 
            : base()
        {
            sprite = new Drawable(spriteTexture, position);
        }

        #endregion

        #region methods

        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
        #endregion




    }
    /// <summary>
    /// This contains all the info of a room on a ship 
    /// </summary>
    class Room : Entity
    {
        /// <summary>
        /// grid position of the room's top-left grid
        /// </summary>
        private Vector2 roomPosition;

        public Vector2 RoomPosition
        {
            get
            {
                return roomPosition;
            }

            set
            {
                RoomPosition = value;
            }
        }

        public Room()
        {

        }

        
    }


    class Ship : Entity
    {
        #region fields
        private int hp;
        private int shields;
        private int energy;
        private List<Grid> shipGrid;
        private Texture2D gridTexture;

        public int HP
        {
            get
            {
                return hp;
            }

            set
            {
                hp = value;
            }
        }

        public int Shields
        {
            get
            {
                return shields;
            }
            set
            {
                shields = value;
            }
        }

        public int Energy
        {
            get
            {
                return energy;
            }
            set
            {
                energy = value;
            }
        }

        private Drawable sprite;

        public Drawable Sprite
        {
            get
            {
                return sprite;
            }

            set
            {
                sprite = value;
            }
        }

        public List<Grid> ShipGrid
        {
            get
            {
                return shipGrid;
            }

            set
            {
                shipGrid = value;
            }
        }

        #endregion

        #region constructors / destructors

        public Ship(Texture2D shipTexture, Texture2D gridTexture, Vector2 position)
            : base()
        {
            hp = 10;
            energy = 5;
            shields = 2;

            sprite = new Drawable(shipTexture, position);
            shipGrid = new List<Grid>();

            for (int i = 0; i < shipTexture.Bounds.Width; i += 32)
            {
                for (int j = 0; j < shipTexture.Bounds.Height; j += 32)
                {
                    Grid newGrid = new Grid(gridTexture, new Vector2(i+position.X, j+position.Y));
                    shipGrid.Add(newGrid);
                }
            }

        }

        #endregion

        #region methods

        override public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        override public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
            foreach (Grid shipgrid in shipGrid)
            {
                shipgrid.Draw(spriteBatch);
            }

            base.Draw(spriteBatch);
        }


        #endregion 
    }
}
