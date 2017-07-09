private Vector2 mapSquareToCircle(Vector2 input)
{
	// map square input to circle, to maintain uniform speed in all directions
	return new Vector2(
		input.x * Mathf.Sqrt(1 - input.y * input.y * 0.5f),
		input.y * Mathf.Sqrt(1 - input.x * input.x * 0.5f)
	);
}

// get input
var input = new Vector2(
	Input.GetAxisRaw("Horizontal"),
	Input.GetAxisRaw("Vertical")
);

var inputMapped = mapSquareToCircle(input);
