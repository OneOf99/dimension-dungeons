using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{

    public Texture[] textures;
    public GameObject cube;

    public struct Tile {
        public int type;
        public GameObject obj;
    }

    public bool build;
    public int SIZE;
    public Tile[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        SIZE = 20;
        grid = new Tile[SIZE,SIZE];
        build = true;

    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.R)){
            build = true;
        }

        if(build) {

            // Delete the previous dungeon
            deleteDungeon();
            // Generate a new dungeon
            //
            //     -- TODO --
            //
            // Build the new dungeon
            buildDungeon();
            
            build = false;
        }
    }

    void buildDungeon() {
        // Build the dungeon
        for (int z=0; z<SIZE; z++) {
            for (int x=0; x<SIZE; x++) {
                grid[z,x].type = Random.Range(0,2);
                
                Texture temp_texture = textures[0];
                int height = 0;
                switch (grid[z,x].type) {
                    case 0:
                        temp_texture = textures[2];
                        height = -1;
                        break;
                    case 1:
                        temp_texture = textures[1];
                        height = 0;
                        break;
                }
                GameObject new_obj = Instantiate(cube,new Vector3(x,height,z), Quaternion.Euler(0,0,0));
                Renderer rend = new_obj.GetComponent<MeshRenderer> ();
                rend.material.SetTexture("_MainTex",temp_texture);
                grid[z,x].obj = new_obj;
            }
        }
    }

    void deleteDungeon() {
        for (int z=0; z<SIZE; z++) {
            for (int x=0; x<SIZE; x++) {
                Destroy(grid[z,x].obj);
            }
        }
    }
}
