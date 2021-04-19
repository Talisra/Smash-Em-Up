using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDictionary
{
    /* This dictionary defines the wave number (int) and the actual enemies
     * Each member of the list is an EnemyBox - means that a list of size(3) will have
     * 2 EnemyBoxes, while the dictionary defines the number of enemies per box.
     * 
     */
    public static Dictionary<int, List<Dictionary<string, int>>> Waves =
        new Dictionary<int, List<Dictionary<string, int>>>()
        {
            {
                1, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 1 }
                    }
                }
            },
            {
                2, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 2 }
                    }
                }
            },
            {
                3, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 1 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 1 }
                    }
                }
            },
            {
                4, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 2 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 1 }
                    }
                }
            },
            {
                5, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 3 }
                    }
                }
            },
            {
                6, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 2 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 1 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 1 }
                    }
                }
            },
            {
                7, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 1 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 1 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 1 }
                    }
                }
            },
            {
                8, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 1 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 2 }
                    }
                }
            },
            {
                9, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 1 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 2 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 1 }
                    }
                }
            },
            {
                10, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 2 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 2 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 1 }
                    }
                }
            },
        };
}
