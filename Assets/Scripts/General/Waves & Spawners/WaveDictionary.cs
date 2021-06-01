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
                        { "Boule", 1 }
                    }
                }
            },
            {
                2, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Boule", 2 }
                    }
                }
            },
            {
                3, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Boule", 2 }
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
                        { "Boule", 3 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 1 }
                    }
                }
            },
            {
                5, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Boule", 4 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 2 }
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
                        { "Boule", 10 }
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
                        { "Cryser", 1 }
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
                        { "Cryser", 1 }
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
                        { "Cuball", 2 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 2 }
                    }
                }

            },
            {
                11, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 2 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 3 }
                    }
                }
            },
            {
                12, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 3 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 1 }
                    }
                }

            },
            {
                13, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 3 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 2 }
                    }
                }

            },
            {
                14, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 2 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 2 }
                    }
                }

            },
            {
                15, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 3 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 3 }
                    }
                }

            },
            {
                16, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 3 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 4 }
                    }
                }

            },
            {
                17, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 3 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 3 }
                    }
                }

            },
            {
                18, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 4 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 4 }
                    }
                }

            },
            {
                19, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 3 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 3 }
                    }
                }

            },
            {
                20, new List<Dictionary<string, int>>() {
                    new Dictionary<string, int>()
                    {
                        { "Cuball", 4 }
                    },
                    new Dictionary<string, int>()
                    {
                        { "Cryser", 4 }
                    }
                }

            },
        };
}
