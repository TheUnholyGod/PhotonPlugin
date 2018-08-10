using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    public static class MapPartitioningButNotPartitioningIsJustSplittingUpTheMap
    {
        public static int
        mapsize = 10;

        public static int ReturnTileViaPos(float posxinput, float poszinput)
        {
            float posx, posz;
            float dividor = 1.0f / mapsize;

            posx = posxinput * dividor;
            posz = poszinput * dividor;

            return (mapsize * ((((int)(posz)) < 0) ? 0 : (int)(posz))) + (int)(posx);
        }

        public static List<int> ReturnSurroundingTilesViaCurrentTile(int tile)
        {
            List<int> temp = new List<int>();

            int tile_selected = 0;
            int min_max_y = 0, min_y = 0, max_y = 0, temp_adder = 0;

            while (true)
            {
                if ((temp_adder * mapsize) > (tile - mapsize) && (temp_adder * mapsize) < (tile + mapsize))
                {
                    min_max_y = temp_adder * mapsize;
                    min_y = ((temp_adder - 2) * mapsize) + (mapsize - 1);
                    max_y = (temp_adder + 2) * mapsize;
                    break;
                }
                ++temp_adder;
            }

            for (int i = -(mapsize + 1); i <= -(mapsize - 1); ++i)
            {
                tile_selected = tile + i;
                if (tile_selected >= 0 && tile_selected < (mapsize * mapsize) && tile_selected < min_max_y && tile_selected > min_y)
                {
                    temp.Add(tile_selected);
                }
            }

            for (int i = (mapsize + 1); i >= (mapsize - 1); --i)
            {
                tile_selected = tile + i;
                if (tile_selected >= 0 && tile_selected < (mapsize * mapsize) && tile_selected > min_max_y + (mapsize - 1) && tile_selected < max_y)
                {
                    temp.Add(tile_selected);
                }
            }

            for (int i = -1; i < 2; ++i)
            {
                tile_selected = tile + i;
                if (tile_selected >= 0 && tile_selected < (mapsize * mapsize) && tile_selected >= min_max_y && tile_selected <= min_max_y + (mapsize - 1) && tile_selected > min_y && tile_selected < max_y)
                {
                    temp.Add(tile_selected);
                }
            }

            return temp;
        }
    }
}
