sprite = "collectibles_172_sacrificialdagger.png"
extraSprites = {}

daggers = {}
angle = 0
radius = 1.5
angularSpeed = 80

function OnEquipped()

	for i = 1, 3 do		
		daggers[i] = AddChild()
		SetComponent(daggers[i], "SpriteRenderer", {sprite = 0})
	end	
end

function Update()
	
	local dt = GetDT()

	angle = angle + dt * angularSpeed

	if angle > 360 then
		angle = angle - 360
	end

	IsaacPos = GetPosition(0)

	-- Rotate the vector (1, 0) angle degrees
	dir = Rotate2DVector({x = 1, y = 0}, angle)
	SetPosition(daggers[1], CalculatePosition(IsaacPos, dir, radius))

	dir = Rotate2DVector({x = 1, y = 0}, angle + 120)
	SetPosition(daggers[2], CalculatePosition(IsaacPos, dir, radius))

	dir = Rotate2DVector({x = 1, y = 0}, angle + 240)
	SetPosition(daggers[3], CalculatePosition(IsaacPos, dir, radius))

end

function Rotate2DVector(vec, degAngle)

	return({x = cos(degAngle) * vec.x - sin(degAngle) * vec.y, y = sin(degAngle) * vec.x + cos(degAngle) * vec.y})

end

function cos(deg)

	return math.cos(math.rad(deg))

end

function sin(deg)

	return math.sin(math.rad(deg))

end

function CalculatePosition(origin, dir, radius)

	return {x = origin.x + dir.x * radius, y = origin.y + dir.y * radius}

end

return
{
	OnEquipped = OnEquipped,
	Update = Update
}