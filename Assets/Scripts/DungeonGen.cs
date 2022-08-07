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
        public int var;
    }

    public struct Coor  {
        public int z;
        public int x;
        public Coor(int z, int x) {
            this.z = z;
            this.x = x;
        }
    }

    public Coor[] points;
    public int pMax;
    public bool build;
    public int SIZE;
    public Tile[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        SIZE = 32;
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
            generateDungeon();
            // Build the new dungeon
            buildDungeon();
            
            build = false;
        }
    }

    //
    // Skeleton for dungeon generation
    void generateDungeon() {
        // Generate points
        points = generatePoints();
        // Randomize points
        //points = shufflePoints(points);
        // Carve paths
        carvePaths(points);
        // Create rivers if needed
        // Fortify border
    }

    Coor[] generatePoints() {
        pMax = SIZE/3 + Random.Range(-1,2);
        Coor[] new_points = new Coor[pMax];
        for (int i=0; i<pMax; i++) {
            int new_z = Random.Range(1,SIZE);
            int new_x = Random.Range(1,SIZE);
            new_points[i].z = new_z;
            new_points[i].x = new_x;
        }
        return new_points;
    }
    
    // Who knows if this works, not me lol
    Coor[] shufflePoints(Coor[] new_points) {
        int[] order = new int[pMax];
        for (int i=0; i<pMax; i++) {
            order[i] = Random.Range(-1000,1000);
        }
        bool sorted = false;
        while(!sorted) {
            sorted = true;
            for (int i=0; i<pMax-1; i++) {
                for (int j=i+1; j<pMax; j++) {
                    if (order[i] > order[j]) {
                        // swap
                        int tmp_int = order[i];
                        Coor tmp_coor = new_points[i];
                        order[i] = order[j];
                        new_points[i] = new_points[j];
                        order[j] = tmp_int;
                        new_points[j] = tmp_coor;
                        sorted = false;
                    }
                }
            }
        }

        return new_points;
    }

    void place(int Z, int X, int T) {
        if (grid[Z,X].type != 0) {
            grid[Z,X].type = T;
        }
    }

    void carvePaths(Coor[] points) {
        for (int i=0; i<pMax-1; i++) {
            Coor C1 = points[i];
            Coor C2 = points[i+1];
            int sz = C1.z;
            int sx = C1.x;
            int fz = C2.z;
            int fx = C2.x;
            int zd = sign(fz-sz);
            int xd = sign(fx-sx);
            int cz = sz;
            int cx = sx;
            // Carve out path with some randomness, should refine later
            place(cz,cx, 0);
            while (cz != fz || cx != fx) {
                if (cx == fx || (Random.Range(0,2) == 0 &&  cz != fz)) {
                    cz += zd;
                } else {
                    cx += xd;
                }
                place(cz,cx,0);
            }

        }
    }

    void buildDungeon() {
        // Build the dungeon
        for (int z=0; z<SIZE; z++) {
            for (int x=0; x<SIZE; x++) {
                //grid[z,x].type = Random.Range(0,2);
                
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
                grid[z,x].type = 1;
                grid[z,x].var = 0;
            }
        }
    }

    int sign(int x) {
        if (x == 0) {
            return 0;
        } else {
            return (int) Mathf.Sign((float) x);
        }
    }
}
