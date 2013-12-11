﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;

namespace SpaceIsFun
{
    public partial class Game1 : Game
    {

        /// <summary>
        /// sets up the game battle state
        /// </summary>
        /// 


        bool multiSelecting = false;

        Point selectRectStart = new Point();
        Point selectRectEnd = new Point();

        List<int> currentEnemyShips = new List<int>();

        void setupBattle(int playerUID)
        {
            Vector2 target1 = new Vector2();

            Vector2 target2 = new Vector2();

            Vector2 target1Screen = new Vector2();
            Vector2 target2Screen = new Vector2();
            List<int> selectedCrewMembers = new List<int>();
            // mapping of gridID to true or false; whether or not this grid has a crew in it 
            Dictionary<int, bool> gridFilled = new Dictionary<int, bool>();

            bool target1Selected = false;
            bool target2Selected = false;

            Ship playerShip = (Ship)ShipManager.RetrieveEntity(playerUID);


            Pathfinder pather = new Pathfinder(playerShip.ShipGrid, GridManager);

            // sets up seven energy bars for the ship
            Panel energy1 = new Panel(4, screenHeight - 128, 40, 128 - 8);
            Panel energy2 = new Panel(64 + 4, screenHeight - 128, 40, 128 - 8);
            Panel energy3 = new Panel(128 + 4, screenHeight - 128, 40, 128 - 8);
            Panel energy4 = new Panel(192 + 4, screenHeight - 128, 40, 128 - 8);
            Panel energy5 = new Panel(256 + 4, screenHeight - 128, 40, 128 - 8);
            Panel energy6 = new Panel(320 + 4, screenHeight - 128, 40, 128 - 8);
            Panel energy7 = new Panel(384 + 4, screenHeight - 128, 40, 128 - 8);

            /*Lance's code*/
            Panel Health = new Panel(5, 5, (32 * playerShip.MaxHP) + 8, 72);
            /*END Lance's code*/

            Image energyBar1;
            Image HealthBar;

            // this list will hold the individual bars within one energy bar
            List<Widget> energyBarTest = new List<Widget>();
            List<Widget> healthBarTest = new List<Widget>();

            int shipStartEnergy = playerShip.Energy;

            Point currentlySelectedPlayerGrid = new Point(-1, -1);
            Point currentlySelectedEnemyGrid = new Point(-1, -1);

            StateMachine cursorState = new StateMachine();

            State idleCursor = new State { Name = "idleCursor" };
            State hasSelectedCrew = new State { Name = "hasSelectedCrew" };
            State targetWeapon = new State { Name = "targetWeapon" };

            idleCursor.Transitions.Add(hasSelectedCrew.Name, hasSelectedCrew);
            idleCursor.Transitions.Add(targetWeapon.Name, targetWeapon);
            hasSelectedCrew.Transitions.Add(idleCursor.Name, idleCursor);
            hasSelectedCrew.Transitions.Add(hasSelectedCrew.Name, hasSelectedCrew);



            cursorState.Start(idleCursor);

            #region battle state methods
            // when entering the battle state
            #region battle state enter
            battle.enter += () =>
            {
                // add gui elements here

                // player ship HP, enemy ship HP


                // player ship shields, enemy ship shields


                // player ship total / current energy (we'll deal with segmented energy later)

                // player ship rooms: 
                // powered up or down, on fire, hull breach


                // weapons GUI: 
                // weapon slots 1-5 for each:
                // filled or not, enabled or not, charging or not

                // adds all the energy bars to the gui
                gui.AddWidget(energy1);
                gui.AddWidget(energy2);
                gui.AddWidget(energy3);
                gui.AddWidget(energy4);
                gui.AddWidget(energy5);
                gui.AddWidget(energy6);
                gui.AddWidget(energy7);

                /*Lance's*/
                gui.AddWidget(Health);

                Texture2D HB;

                
                if (playerShip.CurrentHP > 3)
                    for (int i = 0; i < 3; i++)
                    {
                        HB = healthBarLow; // red healthbar
                        Health.AddWidget(HealthBar = new Image(32 * i, 0, HB));
                        healthBarTest.Add(HealthBar);
                    }
                if (playerShip.CurrentHP > 6)
                    for (int i = 0; i < 6; i++)
                    {
                        HB = healthBarMed; // orange healthbar
                        Health.AddWidget(HealthBar = new Image(32 * i, 0, HB));
                        healthBarTest.Add(HealthBar);
                    }
                for (int i = 0; i < playerShip.CurrentHP; i++)
                {
                    HB = healthBarFull; // green healthbar
                    Health.AddWidget(HealthBar = new Image(32 * i, 0, HB));
                    healthBarTest.Add(HealthBar);
                }
                /*END Lance's*/

                // add as many energy widgets as there is ship energy to one entire energy bar
                for (int i = 0; i < playerShip.Energy; i++)
                {
                    energy1.AddWidget(energyBar1 = new Image(0, (128 - 16 - 8 - 8) - i * 16, energyBarSprite));
                    energyBarTest.Add(energyBar1);
                }

                currentEnemyShips.Add(enemyShipUID);
            };

            #endregion
            // when updating the battle state
            #region battle state update
            battle.update += (GameTime gameTime) =>
            {
                #region input handling

                #region keys

                Ship thisShip = (Ship)ShipManager.RetrieveEntity(0);

                if (currentKeyState.IsKeyDown(Keys.T) && previousKeyState.IsKeyUp(Keys.T))
                {
                    Crew man = (Crew)CrewManager.RetrieveEntity(0);

                    pather = new Pathfinder(thisShip.ShipGrid, GridManager);

                    Grid thisGrid = (Grid)GridManager.RetrieveEntity(9);

                    target1 = man.Position;

                    thisGrid = (Grid)GridManager.RetrieveEntity(26);

                    target2 = thisGrid.GridPosition;

                    List<Vector2> path = pather.FindOptimalPath(target1, target2);

                    foreach (Vector2 item in path)
                    {
                        Vector2 dumb = new Vector2((item.X / 32), (item.Y / 32));

                        System.Diagnostics.Debug.WriteLine(dumb.ToString());
                    }

                    man.Move(path);
                }

                // if the a key is pressed, transition back to the menu
                if (currentKeyState.IsKeyDown(Keys.A))
                {
                    stateMachine.Transition(startMenu.Name);
                }

                if (currentKeyState.IsKeyDown(Keys.D1) && previousKeyState.IsKeyUp(Keys.D1))
                {
                    Weapon thisWeapon = (Weapon)WeaponManager.RetrieveEntity(playerShip.WeaponSlots[0]);

                    // if weapon 0 is currently disabled
                    if (thisWeapon.weaponStateMachine.CurrentState.Name == "disabled")
                    {
                        // enable the weapon if possible

                        // start charging
                    }

                    // if weapon 0 is charging
                    else if (thisWeapon.weaponStateMachine.CurrentState.Name == "charging")
                    {
                        // go to target weapon state, try to assign target
                    }


                }

                if (currentKeyState.IsKeyDown(Keys.D2) && previousKeyState.IsKeyUp(Keys.D2))
                {
                    // if weapon 1 is not charging, try to assign energy, start charging, go to weapon target cursor state, and target enemy

                    // if weapon 1 is ready, go to weapon target cursor state, and target enemy
                }

                if (currentKeyState.IsKeyDown(Keys.D3) && previousKeyState.IsKeyUp(Keys.D3))
                {
                    // if weapon 2 is not charging, try to assign energy, start charging, go to weapon target cursor state, and target enemy

                    // if weapon 2 is ready, go to weapon target cursor state, and target enemy
                }

                if (currentKeyState.IsKeyDown(Keys.D4) && previousKeyState.IsKeyUp(Keys.D4))
                {
                    // if weapon 3 is not charging, try to assign energy, start charging, go to weapon target cursor state, and target enemy

                    // if weapon 3 is ready, go to weapon target cursor state, and target enemy
                }

                if (currentKeyState.IsKeyDown(Keys.D5) && previousKeyState.IsKeyUp(Keys.D5))
                {
                    // if weapon 4 is not charging, try to assign energy, start charging, go to weapon target cursor state, and target enemy

                    // if weapon 4 is ready, go to weapon target cursor state, and target enemy
                }



                #region keys.c
                /*
                // if the c key is tapped, query to see if the cursor is hovering over the ship
                if (currentKeyState.IsKeyDown(Keys.C) && previousKeyState.IsKeyUp(Keys.C))
                {
                    // returns whether or not the cursor is hovering over the player's ship
                    int shipUID = checkShipHover(currentMouseState);
                    


                    // if the cursor is hovering over the player's ship, print a message and figure out which room the cursor is in
                    if (shipUID == playerShipUID)
                    {
                        System.Diagnostics.Debug.WriteLine("Cursor on player ship!");
                        

                        // returns which grid (in ship grid coords) the cursor is hovering over
                        Vector2 gridHover = getGridHover(currentMouseState, shipUID);


                        // if gridHover isn't (-1,-1), which means the cursor ISNT on the grid, print messages, and highlight (or un-highlight) that grid 
                        if (gridHover.X != -1 && gridHover.Y != -1)
                        {
                            Ship thisShip = (Ship)ShipManager.RetrieveEntity(shipUID);

                            int gridUID = thisShip.ShipGrid[(int)gridHover.X, (int)gridHover.Y];

                            Grid thisGrid = (Grid)GridManager.RetrieveEntity(gridUID);

                            System.Diagnostics.Debug.WriteLine("Cursor on grid: " + thisGrid.GridPosition.ToString());
                            
                            if (checkRoomHover(currentMouseState, shipUID) == true)
                            {
                                int roomUID = getRoomHover(currentMouseState, shipUID);

                                Room thisRoom = (Room)RoomManager.RetrieveEntity(roomUID);

                                System.Diagnostics.Debug.WriteLine("Cursor on room: " + thisRoom.RoomPosition.ToString());
                            }

                            //highlight the grid
                            if (target1Selected == false)
                            {
                                if (thisGrid.IsWalkable == true)
                                {
                                    target1Selected = true;
                                }

                            }

                            target1 = thisGrid.GridPosition;
                            target1Screen = new Vector2(currentMouseState.X, currentMouseState.Y);
                            

                        }
                    }


                }

                #endregion

                #region keys.v
                // second target for pathfinder checking 

                if (currentKeyState.IsKeyDown(Keys.V) && previousKeyState.IsKeyUp(Keys.V))
                {
                    // returns whether or not the cursor is hovering over the player's ship
                    int shipUID = checkShipHover(currentMouseState);



                    // if the cursor is hovering over the player's ship, print a message and figure out which room the cursor is in
                    if (shipUID == playerShipUID)
                    {
                        // returns which grid (in ship grid coords) the cursor is hovering over
                        Vector2 gridHover = getGridHover(currentMouseState, shipUID);

                        Ship thisShip = (Ship)ShipManager.RetrieveEntity(shipUID);

                        if (target2Selected == false && target1Selected == true)
                        {
                            Grid thisGrid = (Grid)GridManager.RetrieveEntity(thisShip.ShipGrid[(int)gridHover.X, (int)gridHover.Y]);
                            if (thisGrid.IsWalkable == true)
                            {
                                
                                target2 = thisGrid.GridPosition;
                                target2Screen = new Vector2(currentMouseState.X, currentMouseState.Y);

                                System.Diagnostics.Debug.WriteLine(target1.ToString() + " " + target2.ToString());

                                pather = new Pathfinder(thisShip.ShipGrid, GridManager);

                                List<Vector2> path = pather.FindOptimalPath(target1, target2);

                                foreach (Vector2 item in path)
                                {
                                    Vector2 dumb = new Vector2((item.X / 32), (item.Y / 32));

                                    System.Diagnostics.Debug.WriteLine(dumb.ToString());
                                }

                                double c = Math.Sqrt(Math.Pow((double)(target2Screen.X - target1Screen.X), 2d) + Math.Pow((double)(target2Screen.Y - target1Screen.Y), 2d));

                                System.Diagnostics.Debug.WriteLine(c.ToString());

                                List<Vector2> pathList = new List<Vector2>();

                                pathList.Add(target2Screen);

                                target1Selected = false;

                                target1 = new Vector2();
                                target2 = new Vector2();
                            }
                        }
                    }
                }
                */
                #endregion

                #region keys.e
                // if the e key is tapped, try to lose energy if possible
                if (currentKeyState.IsKeyDown(Keys.E) == true && previousKeyState.IsKeyUp(Keys.E) == true)
                {
                    if (playerShip.Energy > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("lost energy!");
                        playerShip.Energy = playerShip.Energy - 1;
                    }

                    // iterate over the energy widgets for the first energy bar, and make the current level invisible
                    for (int i = 0; i < shipStartEnergy; i++)
                    {
                        if (i >= playerShip.Energy)
                        {
                            energyBarTest[i].Visible = false;
                        }
                    }

                }

                #endregion

                #region keys.r
                // if the f key is tapped, try to gain energy if possibile
                if (currentKeyState.IsKeyDown(Keys.R) == true && previousKeyState.IsKeyUp(Keys.R) == true)
                {
                    if (playerShip.Energy < 5)
                    {
                        System.Diagnostics.Debug.WriteLine("gained energy!");
                        playerShip.Energy = playerShip.Energy + 1;
                    }

                    // iterate over the energy widgets for the first energy bar, and make the one above the current level visible again
                    for (int i = 0; i < shipStartEnergy; i++)
                    {
                        if (i < playerShip.Energy)
                        {
                            energyBarTest[i].Visible = true;
                        }
                    }
                }

                #endregion

                #region keys.o
                if (currentKeyState.IsKeyDown(Keys.O) == true && previousKeyState.IsKeyUp(Keys.O) == true)
                {
                    if (playerShip.CurrentHP < 10)
                    {
                        System.Diagnostics.Debug.WriteLine("gained energy!");
                        playerShip.CurrentHP = playerShip.CurrentHP + 1;
                    }

                    int j = 0;
                    for (int i = 0; i < 19; i++)
                    {
                        if (playerShip.CurrentHP > 6)
                            j = 9;
                        else if (playerShip.CurrentHP > 3)
                            j = 3;
                        else
                            j = 0; 
                        
                        if (i < playerShip.CurrentHP + j)
                        {
                            healthBarTest[i].Visible = true;
                        }
                    }
                }
                #endregion

                #region keys.p
                if (currentKeyState.IsKeyDown(Keys.P) == true && previousKeyState.IsKeyUp(Keys.P) == true)
                {
                    if (playerShip.CurrentHP > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("lost energy!");
                        playerShip.CurrentHP = playerShip.CurrentHP - 1;
                    }

                    int j = 0;
                    for (int i = 0; i < 19; i++)
                    {
                        if (playerShip.CurrentHP > 6)
                            j = 9; 
                        else if (playerShip.CurrentHP > 3)
                            j = 3;
                        else
                            j = 0;

                        if (i >= playerShip.CurrentHP + j)
                        {
                            healthBarTest[i].Visible = false;
                        }
                    }
                }
                #endregion

                #region weapons testing: keys.y, keys.u
                //a test to see if my states work -Peter
                if (currentKeyState.IsKeyDown(Keys.Y) == true)
                {
                    Weapon defaultWeapon = (Weapon)WeaponManager.RetrieveEntity(playerShip.WeaponSlots[0]);



                    defaultWeapon.EnoughPower = true;
                    defaultWeapon.start_charging();
                    defaultWeapon.weaponStateMachine.Update(gameTime);
                    //System.Diagnostics.Debug.WriteLine(defaultWeapon.weaponStateMachine.CurrentState.Name);
                }

                //a test to see if my states work -Peter
                if (currentKeyState.IsKeyDown(Keys.U) == true)
                {
                    Weapon defaultWeapon = (Weapon)WeaponManager.RetrieveEntity(playerShip.WeaponSlots[0]);

                    defaultWeapon.deactivate_weap();
                    defaultWeapon.weaponStateMachine.Update(gameTime);
                }

                #endregion

                #endregion

                #region mouse




                #endregion
                #endregion
            #endregion

                ShipManager.Update(gameTime);
                GridManager.Update(gameTime);
                RoomManager.Update(gameTime);
                WeaponManager.Update(gameTime);

                cursorState.Update(gameTime);

            };


            // when leaving the battle state
            #region battle state leave
            battle.leave += () =>
            {
                // tear down gui elements

                // remove the energy widgets from the gui
                gui.RemoveWidget(energy1);
                gui.RemoveWidget(energy2);
                gui.RemoveWidget(energy3);
                gui.RemoveWidget(energy4);
                gui.RemoveWidget(energy5);
                gui.RemoveWidget(energy6);
                gui.RemoveWidget(energy7);

                foreach (int UID in currentEnemyShips)
                {
                    ShipManager.DeleteEntity(UID);
                }
            };
            #endregion
            #endregion

            #region idle cursor state methods
            idleCursor.enter += () =>
            {
            };

            idleCursor.update += (GameTime gameTime) =>
            {
                #region input handling

                

                #region mouse

                #region left click

                // if we were previously holding the mouse button down, but now its released
                if (previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                {
                    // if there is a crew in the current cursor's grid, and we are not multiselecting, select that crew member, transition to hasSelectedCrew
                    if (multiSelecting == false)
                    {
                        //cursorState.Transition(hasSelectedCrew.Name);
                    }
                    // else if we are multiselecting: get (x1,y1;x2,y2), select all crew in that area, set multiselecting to false, transition to hasSelectedCrew if any crew were selected

                    // note: this should only happen if the two points are on the player's ship
                    else if (multiSelecting == true && checkShipHover(selectRectStart) == playerShipUID && checkShipHover(selectRectEnd) == playerShipUID)
                    {
                        int x1 = (selectRectStart.X - 50) / 32;
                        int y1 = (selectRectStart.Y - 50) / 32;
                        int x2 = (selectRectEnd.X - 50) / 32;
                        int y2 = (selectRectEnd.Y - 50) / 32;

                        // swap points if they need to be
                        int temp;

                        if (x2 < x1)
                        {
                            temp = x2;
                            x2 = x1;
                            x1 = temp;
                        }

                        if (y2 < y1)
                        {
                            temp = y2;
                            y2 = y1;
                            y1 = temp;
                        }

                        x1 = Math.Max(x1, 0);
                        y1 = Math.Max(y1, 0);

                        x2 = Math.Min(x2, playerShip.ShipGrid.GetLength(0) - 1);
                        y2 = Math.Min(y2, playerShip.ShipGrid.GetLength(1) - 1);



                        System.Diagnostics.Debug.WriteLine("x1, y1 {0},{1}", x1, y1);
                        System.Diagnostics.Debug.WriteLine("x2, y2 {0},{1}", x2, y2);

                        for (int i = x1; i <= x2; i++)
                        {
                            for (int j = y1; j <= y2; j++)
                            {
                                System.Diagnostics.Debug.WriteLine("Selected Grid {0},{1}", i, j);

                                var crewMembers = CrewManager.RetrieveKeys();

                                foreach (int k in crewMembers)
                                {
                                    Crew thisguy = (Crew)CrewManager.RetrieveEntity(k);

                                    if (thisguy.Position.X == i && thisguy.Position.Y == j)
                                    {
                                        thisguy.Selected = true;

                                        selectedCrewMembers.Add(k);
                                    }
                                }
                            }
                        }

                        if (selectedCrewMembers.Count > 0)
                        {
                            cursorState.Transition(hasSelectedCrew.Name);
                        }
                    }

                    multiSelecting = false;

                   
                }

                // if we're holding the mouse button down
                if (previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    // if we arent multiselecting: set multiselecting to true, start point = previous cursor's position; end point = current cursor's position
                    if (multiSelecting == false)
                    {
                        multiSelecting = true;
                        selectRectStart.X = previousMouseState.X;
                        selectRectStart.Y = previousMouseState.Y;

                        selectRectEnd.X = currentMouseState.X;
                        selectRectEnd.Y = currentMouseState.Y;
                    }

                    else if (multiSelecting == true)
                    {
                        selectRectEnd.X = currentMouseState.X;
                        selectRectEnd.Y = currentMouseState.Y;
                    }
                    // else if we are multiselecting: end point = current cursor's position
                }

                #endregion


                #region right click
                if (previousMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed)
                {
                    // if we've rightclicked

                    // gui interaction: 

                    // if we've right-clicked over a weapon gui element (the charge bar)
                    /*
                    if(rightclick position == weapon 0 space)
                    {
                        //disable weapon
                        thisWeapon = (Weapon)WeaponManager.RetrieveEntity(playerShip.WeaponSlots[0]);
                        thisWeapon.WeaponStateMachine.Transition(disabled.Name);

                    }
                    */
                }

                #endregion

                #endregion

                #endregion
            };

            idleCursor.leave += () =>
            {
            };
            #endregion

            #region selected crew cursor state methods
            hasSelectedCrew.enter += () =>
            {
            };

            hasSelectedCrew.update += (GameTime gameTime) =>
            {
                #region input handling

                


                #region mouse

                if (previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    // if we've leftclicked

                    //deselect the crew, go to idlecursor

                    

                    selectedCrewMembers.Clear();
                    cursorState.Transition(idleCursor.Name);
                }

                if (previousMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed)
                {
                    // if we've rightclicked

                    // move the crew, we can assume the selected crew list has size of at least 1

                    if (selectedCrewMembers.Count == 1)
                    {
                        // we only have one man

                        // get the room we clicked, and if its on the ship, check if the room is empty of mans

                        // if it has room, move the man to an empty grid in that room

                        // transition to idle cursor on success

                        // todo: room-filling algorithm



                    }

                    else
                    {
                        // we got more than one man

                        // did we click on a room on our ship?
                        if (checkShipHover(currentMouseState) == playerShipUID)
                        {
                            if (checkRoomHover(currentMouseState, playerShipUID) == true)
                            {
                                int thisRoomUID = getRoomHover(currentMouseState, playerShipUID);
                                Room thisRoom = (Room)RoomManager.RetrieveEntity(getRoomHover(currentMouseState, playerShipUID));
                                //check if the room has enough room to move the entire list of selected mans

                                // loop through CrewToRoom, count any hits in the values; if count is less than or equal to the room's size then continue
                                int count = 0;
                                foreach (var item in CrewToRoom.Values)
                                {
                                    if (thisRoomUID == item)
                                    {
                                        count++;
                                    }
                                }

                                if (count <= thisRoom.RoomSize)
                                {
                                    // at this point, we can move the crew to the room
                                    // todo: room-filling algorithm
                                    // transition to idle cursor on success


                                }
                            }
                        }



                    }


                }

                #endregion
                #endregion
            };

            hasSelectedCrew.leave += () =>
            {

            };
            #endregion

            #region weapon target cursor state methods
            targetWeapon.enter += () =>
            {

            };

            targetWeapon.update += (GameTime gameTime) =>
            {
                #region input handling

                #region mouse

                if (previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    // if we've leftclicked

                    // did we click an enemy room?

                    // if so, get the weapon we're currently selecting

                    // set the enemy room as the weapon's target

                    // transition to idle cursor on success
                }

                if (previousMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed)
                {
                    // if we've rightclicked

                    //Weapon thisWeapon = (Weapon)WeaponManager.RetrieveEntity(playerShip.WeaponSlots[0]);

                    // transition to idle cursor

                    cursorState.Transition("idleCursor");
                }

                #endregion
                #endregion
            };

            targetWeapon.leave += () =>
            {
            };
            #endregion




        }

    }
}