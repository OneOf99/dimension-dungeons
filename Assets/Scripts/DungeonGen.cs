using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{

    public Texture[] textures;
    public GameObject cube;
    public Gameplay CAM;

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
        // Generate a new dungeon
        generateDungeon();
        // Build the new dungeon
        buildDungeon();

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
            
            Debug.Log("built");

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
        carveRooms(points);
        // Create rivers if needed
        // Fortify border
        fortify();
        carveRiverHorz();

        // Set player starting point
        Coor p = points[0];
        CAM.updatePosition(p.z,p.x);
    }

    Coor[] generatePoints() {
        pMax = SIZE/3 + Random.Range(-1,2);
        Coor[] new_points = new Coor[pMax];
        for (int i=0; i<pMax; i++) {
            int new_z = Random.Range(1,SIZE-1);
            int new_x = Random.Range(1,SIZE-1);
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

    void place(int Z, int X, int T, int V = 0) {
        if (grid[Z,X].type == 0 || grid[Z,X].type == 1) {
            grid[Z,X].type = T;
            grid[Z,X].var = V;
        }
    }

    void placeRoom(int Z, int X, int V = 0) {
        place(Z-1,X-1,0,V);
        place(Z-1,X,0,V);
        place(Z-1,X+1,0,V);
        place(Z,X-1,0,V);
        place(Z,X,0,V);
        place(Z,X+1,0,V);
        place(Z+1,X-1,0,V);
        place(Z+1,X,0,V);
        place(Z+1,X+1,0,V);
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

    void carveRooms(Coor[] points) {
        // Get needed data
        Coor rift = points[pMax-1];
        int chest_num = pMax/2 + 1;
        bool chest = false;
        // Create points
        for (int i=1; i<pMax-1; i++) {
            int cz = points[i].z;
            int cx = points[i].x;
            placeRoom(cz,cx);
            if (chest) {
                int tz = cz + Random.Range(-1,2);
                int tx = cx + Random.Range(-1,2);
                place(tz,tx,0,1);
                chest = false;
            } else {
                chest = true;
            }
        }
    }

    void carveRiverHorz() {
        int z = Random.Range((int)(SIZE/4f),(int)(3*SIZE/4f));
        int fz = SIZE-z;
        int x = 0;
        int fx = SIZE-1;
        int dist = (int)Mathf.Abs((float) fz-z);
        int c = 0;
        place(z-1,x,1,1);
        place(z,x,1,1);
        place(z+1,x,1,1);
        while (x < fx) {
            x++;
            c++;
            if (dist != 0 && c == dist) {
                c = 0;
                z += sign(fz-z);
            }
            if (grid[z,x].type == 0) {
                place(z,x,0,1);
            } else {
                place(z,x,1,1);
            }
            if (grid[z-1,x].type == 0) {
                place(z-1,x,0,1);
            } else {
                place(z-1,x,1,1);
            }
            if (grid[z+1,x].type == 0) {
                place(z+1,x,0,1);
            } else {
                place(z+1,x,1,1);
            }
        }
    }

    void buildDungeon() {
        // Build the dungeon
        for (int z=0; z<SIZE; z++) {
            for (int x=0; x<SIZE; x++) {
                //grid[z,x].type = Random.Range(0,2);
                
                Texture temp_texture = textures[0];
                float height = 0f;
                switch (grid[z,x].type) {
                    case 0:
                        switch(grid[z,x].var) {
                            case 0: temp_texture = textures[0]; break;
                            case 1: temp_texture = textures[2]; break;
                        }
                        height = -1f;
                        break;
                    case 1:
                        switch(grid[z,x].var) {
                            case 0: temp_texture = textures[1]; height = Random.Range(-0.45f,-0.1f); break;
                            case 1: temp_texture = textures[3]; height = -1.05f; break;
                        }
                        break;
                }
                GameObject new_obj = Instantiate(cube,new Vector3(x,height,z), Quaternion.Euler(0,0,0));
                new_obj.GetComponent<CubeLogic>().CAM = CAM;
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

    void fortify() {
        for (int i=0; i<SIZE; i++) {
            grid[i,0].type = 1;
            grid[i,0].var = 0;
            grid[i,SIZE-1].type = 1;
            grid[i,SIZE-1].var = 0;
            grid[0,i].type = 1;
            grid[0,i].var = 0;
            grid[SIZE-1,i].type = 1;
            grid[SIZE-1,i].var = 0;
        }
    }

    int sign(int x) {
        if (x == 0) {
            return 0;
        } else {
            return (int) Mathf.Sign((float) x);
        }
    }

    public int getType(int Z, int X) {
        return grid[Z,X].type;
    }
}
