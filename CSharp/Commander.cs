using CruiseControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CruiseControl.Enums;

namespace CruiseControl
{
    public class Commander
    {
        public BoardStatus _currentBoard;
        private const double MIN_HEALTH_BEFORE_REPAIR = .25;

        public Commander()
        {
            _currentBoard = new BoardStatus();
        }

        // Do not alter/remove this method signature
        public List<Command> GiveCommands()
        {
            var cmds = new List<Command>();
            bool moveShipFromWall = false;
            
            //Check board status to see if we need to move any ships
            int teamCounter = 0;
            foreach(var team in _currentBoard.VesselCountPerTeamId)
            {
                if (team.VesselCount > 0)
                    teamCounter++;
            }

            if (_currentBoard.TurnsUntilBoardShrink <= teamCounter)
                moveShipFromWall = true;

            //Loop through each vessel to see if we can issue a command
            foreach (var vessel in _currentBoard.MyVesselStatuses)
            {
                if (vessel.Health > 0 && vessel.MovesUntilRepair==0)
                {
                    if (moveShipFromWall)
                    {
                        var direction = GetDirectionToMoveVesselFromEdge(vessel.Location, _currentBoard.BoardMinCoordinate, _currentBoard.BoardMaxCoordinate);
                        if (direction != String.Empty)
                        {
                            cmds.Add(new Command { vesselid = vessel.Id, action = String.Format(CommandType.Move, direction) });
                            continue;
                        }
                    }

                    //Check if we should move to collide with ship

                    var coordinateToFireMissile = GetCoordinateToFireMissile(vessel.SonarReport);
                    if(coordinateToFireMissile != null)
                        cmds.Add(new Command { vesselid = vessel.Id, action = CommandType.Fire
                            ,coordinate = new Coordinate { X = coordinateToFireMissile.X, Y = coordinateToFireMissile.Y }
                        });
                        
                    if (vessel.Health < (MIN_HEALTH_BEFORE_REPAIR * vessel.MaxHealth))
                    {
                        cmds.Add(new Command { vesselid = vessel.Id, action = CommandType.Repair });
                        continue;
                    }

                    //Check to move
                }
            }

            // Add Commands Here.
            // You can only give as many commands as you have un-sunk vessels. Powerup commands do not count against this number. 
            // You are free to use as many powerup commands at any time. Any additional commands you give (past the number of active vessels) will be ignored.
			cmds.Add(new Command { vesselid = 1, action = CommandType.Fire, coordinate = new Coordinate { X = 1, Y = 1 } });

            return cmds;
        }

        // Do NOT modify or remove! This is where you will receive the new board status after each round.
        public void GetBoardStatus(BoardStatus board)
        {
            _currentBoard = board;
        }

        // This method runs at the start of a new game, do any initialization or resetting here 
        public void Reset()
        {

        }

        private static string GetDirectionToMoveVesselFromEdge(List<Coordinate> vesselCoords, Coordinate boardMin, Coordinate boardMax)
        {
            foreach (var coord in vesselCoords)
            {
                if (coord.X == boardMin.X)
                    return "east";
                if(coord.X == boardMax.X)
                    return "west";
                if (coord.Y > boardMin.Y)
                    return "south";
                if(coord.Y == boardMax.Y)
                    return "north";
            }
            return String.Empty;
        }

        private static Coordinate GetDirectionToMoveVesselIntoCollision(List<Coordinate> sonarReport)
        {
            if (sonarReport.Count > 0)
            { 
                return sonarReport[sonarReport.Count/2];
            }

            return null;
        }

        private static Coordinate GetCoordinateToFireMissile(List<Coordinate> sonarReport)
        {
            if (sonarReport.Count > 0)
            {
                return sonarReport[sonarReport.Count / 2];
            }

            return null;
        }

    }
}