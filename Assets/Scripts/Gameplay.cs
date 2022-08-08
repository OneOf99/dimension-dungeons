using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
	public GameObject cube;
	public DungeonGen d_script;
	public Vector3 VEC;
	public Quaternion QUAT;
	
	private int F_MAX;
	private int F_WAIT;
	
	public int X;
	public int Z;
	private int Xv;
	private int Zv;
	
	// Movement vars
	private bool moving;
	private bool will_move;
	private bool waiting;
	private int move_type;
	public int dir;
	public Vector3 move3;
	public int dir_mod;
	public int count;
	
    void Awake () {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 240;
    }

    // Start is called before the first frame update
    void Start()
    {
		// SET MOVEMENT FRAME LENGTH
		F_MAX = 45;
		F_WAIT = 15;
		
        VEC = transform.position;
		move_type = 0;
		count = 0;
		moving = false;
		waiting = false;
		
		// Set start position
		X = 1;
		Z = 1;
		Xv = F_MAX;
		Zv = F_MAX;
		
    }

    // Update is called once per frame
    void Update()
    {
		if (!waiting) {
			VEC = transform.position;
			QUAT = transform.rotation;
			
			// Get key input
			if (!moving) {
				dir_mod = dir;
				if (Input.GetKey(KeyCode.W)){
					will_move = true;
					move_type = 1;
				} else if (Input.GetKey(KeyCode.S)) {
					will_move = true;
					move_type = 1;
					dir_mod = (dir + 2) % 4;
				} else if (Input.GetKey(KeyCode.Q)) {
					moving = true;
					move_type = -2;
				} else if (Input.GetKey(KeyCode.E)) {
					moving = true;
					move_type = 2;
				} else if (Input.GetKey(KeyCode.A)) {
					will_move = true;
					move_type = 1;
					dir_mod = (dir + 3) % 4;
				} else if (Input.GetKey(KeyCode.D)) {
					will_move = true;
					move_type = 1;
					dir_mod = (dir + 1) % 4;
				} else if (Input.GetKeyDown(KeyCode.Space)) {
					//Instantiate(cube,new Vector3(Mathf.Round(VEC.x),Mathf.Round(VEC.y-1),Mathf.Round(VEC.z)), Quaternion.Euler(0,0,0));
				}
				if (will_move) {
					if (dir_mod == 0 && d_script.getType(Z+1,X) == 1) {
						moving = false;
					} else if (dir_mod == 1 && d_script.getType(Z,X+1) == 1) {
						moving = false;
					} else if (dir_mod == 2 && d_script.getType(Z-1,X) == 1) {
						moving = false;
					} else if (dir_mod == 3 && d_script.getType(Z,X-1) == 1) {
						moving = false;
					} else {
						moving = true;
					}
					will_move = false;
				}
			}
			
			// Perform move fragment on frame
			if (moving) {
				count += 1;
				switch (move_type) {
					case 1:
						switch (dir_mod) {
							case 0:
								Zv += 1;
								break;
							case 1:
								Xv += 1;
								break;
							case 2:
								Zv -= 1;
								break;
							case 3:
								Xv -= 1;
								break;
						}
						VEC = new Vector3((float)Xv/(float)F_MAX,VEC.y,(float)Zv/(float)F_MAX);
						break;
					case 2:
						QUAT = Quaternion.Euler(QUAT.eulerAngles.x,QUAT.eulerAngles.y+(float)90/F_MAX,QUAT.eulerAngles.z);
						break;
					case -2:
						QUAT = Quaternion.Euler(QUAT.eulerAngles.x,QUAT.eulerAngles.y-(float)90/F_MAX,QUAT.eulerAngles.z);
						break;
				}
			}
			
			// End movement if done
			if (count == F_MAX) {
				if (move_type == -2) {
					dir = (dir + 3) % 4;
				} else if (move_type == 2) {
					dir = (dir + 1) % 4;
				}
				X = (int) Xv/F_MAX;
				Z = (int) Zv/F_MAX;
				QUAT = Quaternion.Euler(Mathf.Round(QUAT.eulerAngles.x),Mathf.Round(QUAT.eulerAngles.y),Mathf.Round(QUAT.eulerAngles.z));
				moving = false;
				waiting = true;
				count = F_WAIT;
			}
			
			//Instantiate(cube,new Vector3(VEC.x,0,0), Quaternion.Euler(0,0,0));
			transform.position = VEC;
			transform.rotation = QUAT;
		} else {
			if (count > 0) {
				count--;
			} else {
				waiting = false;
			}
		}
    }

	public void updatePosition(int new_Z, int new_X) {
		Z = new_Z;
		X = new_X;
		Zv = Z*F_MAX;
		Xv = X*F_MAX;
		
		VEC = new Vector3(X,VEC.y,Z);
		transform.position = VEC;
	}
}
