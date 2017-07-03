// get input
var input = new Vector2(
	Input.GetAxisRaw("Horizontal"),
	Input.GetAxisRaw("Vertical")
);

// map square input to circle, to maintain uniform speed in all directions
var inputCircle = new Vector2(
	input.x * Mathf.Sqrt(1 - input.y * input.y * 0.5f),
	input.y * Mathf.Sqrt(1 - input.x * input.x * 0.5f)
);
