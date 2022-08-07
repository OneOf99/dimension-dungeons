using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
	public GameObject cube;
	public Vector3 VEC;
	public Quaternion QUAT;
	
	private int F_MAX;
	
	public int X;
	public int Z;	
	private int Xv;
	private int Zv;
	
	// Movement vars
	private bool moving;
	private int move_type;
	public int dir;
	public Vector3 move3;
	public int dir_mod;
	public int count;
	
    // Start is called before the first frame update
    void Start()
    {
		// SET MOVEMENT FRAME LENGTH
		F_MAX = 40;
		
        VEC = transform.position;
		move_type = 0;
		count = 0;
		moving = false;
		
		// Set start position
		X = 1;
		Z = 1;
		Xv = F_MAX;
		Zv = F_MAX;
		
    }

    // Update is called once per frame
    void Update()
    {
		VEC = transform.position;
		QUAT = transform.rotation;
		
		// Get key input
		if (!moving) {
			dir_mod = dir;
			if (Input.GetKeyDown(KeyCode.W)){
				moving = true;
				move_type = 1;
			} else if (Input.GetKeyDown(KeyCode.S)) {
				moving = true;
				move_type = 1;
				dir_mod = (dir + 2) % 4;
			} else if (Input.GetKeyDown(KeyCode.Q)) {
				moving = true;
				move_type = -2;
			} else if (Input.GetKeyDown(KeyCode.E)) {
				moving = true;
				move_type = 2;
			} else if (Input.GetKeyDown(KeyCode.A)) {
				moving = true;
				move_type = 1;
				dir_mod = (dir + 3) % 4;
			} else if (Input.GetKeyDown(KeyCode.D)) {
				moving = true;
				move_type = 1;
				dir_mod = (dir + 1) % 4;
			} else if (Input.GetKeyDown(KeyCode.Space)) {
				Instantiate(cube,new Vector3(Mathf.Round(VEC.x),Mathf.Round(VEC.y-1),Mathf.Round(VEC.z)), Quaternion.Euler(0,0,0));
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
					QUAT = Quaternion.Euler(QUAT.eulerAngles.x,QUAT.eulerAngles.y+2.25f,QUAT.eulerAngles.z);
					break;
				case -2:
					QUAT = Quaternion.Euler(QUAT.eulerAngles.x,QUAT.eulerAngles.y-2.25f,QUAT.eulerAngles.z);
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
			count = 0;
		}
		
		//Instantiate(cube,new Vector3(VEC.x,0,0), Quaternion.Euler(0,0,0));
		transform.position = VEC;
		transform.rotation = QUAT;
    }
}
