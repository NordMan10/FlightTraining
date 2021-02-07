using FlightTraining.Model.Enums;
using System;
using System.Collections.Generic;

namespace FlightTraining.Model
{
    public static class AircraftOptions
    {
        public static readonly Dictionary<AircraftFlow, Dictionary<AircraftType, Dictionary< int, int?[][]>>> AircraftsTracks = 
            new Dictionary<AircraftFlow, Dictionary<AircraftType, Dictionary<int, int?[][]>>>()
        {
            {
                AircraftFlow.Arrive, new Dictionary<AircraftType, Dictionary<int, int?[][]>>()
                {
                    {
                        AircraftType.Plane, new Dictionary<int, int?[][]>()
                        {
                            { 0, new[] { new int?[] { 0 }, new int?[] { 0 } } },
                            { 1, new[] { new int?[] { 2 }, new int?[] { 0 } } },
                            { 2, new[] { new int?[] { 3 }, new int?[] { 0 } } }
                        }
                    }
                }
            },
            {
                AircraftFlow.Depurture, new Dictionary<AircraftType, Dictionary<int, int?[][]>>()
                {
                    {
                        AircraftType.Plane, new Dictionary<int, int?[][]>()
                        {
                            { 0, new[] { new int?[] { 1 }, new int?[] { 1 } } },
                            { 1, new[] { new int?[] { 1 }, new int?[] { 2 } } },
                            { 2, new[] { new int?[] { 1 }, new int?[] { 3 } } }
                        }
                    }
                }
            },
            {
                AircraftFlow.Passing, new Dictionary<AircraftType, Dictionary<int, int?[][]>>()
                {
                    {
                        AircraftType.Umv, new Dictionary<int, int?[][]>()
                        {
                            { 0, new[] { new int?[] { 0 }, null, new int?[] { 0 } } },
                            { 1, new[] { new int?[] { 0 }, new int?[] { 0 }, new int?[] { 0 } } },
                            { 2, new[] { new int?[] { 0 }, new int?[] { 0, 1 }, new int?[] { 1 } } }
                        }
                    }
                }
            }
        };

        public static readonly Dictionary<AircraftType, int> AircraftVelocities = new Dictionary<AircraftType, int>
        {
            { AircraftType.Plane, 100 },
            { AircraftType.Umv, 50 }
        };

        public static readonly Dictionary<AircraftFlow, Tuple<int, int>> AircraftInterval =
            new Dictionary<AircraftFlow, Tuple<int, int>>
            {
                {AircraftFlow.Arrive, Tuple.Create(20000, 30000)},
                {AircraftFlow.Depurture, Tuple.Create(20000, 40000)},
                {AircraftFlow.Passing, Tuple.Create(30000, 40000)}
            };

        public static readonly Dictionary<AircraftType, int> AircraftsImageSizes = new Dictionary<AircraftType, int>
        {
            { AircraftType.Plane, 40 },
            { AircraftType.Umv, 30 },
        };

        public static readonly int CloseDistance = 3;

        public static int PredictInterval = 45;

        public static Dictionary<int, int> UmvTracksGainHeight = new Dictionary<int, int>()
        {
            { 0, 1100 },
            { 1, 1300 },
            { 2, 1100 }
        };

        public static int TimeToGainHeight = 40;

        public static int ConflictDistance = 2000;

        public static Dictionary<AircraftType, string> Names = new Dictionary<AircraftType, string>
        {
            { AircraftType.Plane, "A" },
            { AircraftType.Umv, "UMV" }
        };

        public static Dictionary<AircraftType, string> ImagePaths = new Dictionary<AircraftType, string>
        {
            { AircraftType.Plane, @"images\plane.png" },
            { AircraftType.Umv, @"images\quadrocopter.png" }
        };
    }
}
