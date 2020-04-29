sprite = "collectibles_172_sacrificialdagger.png"
extraSprites = {}

daggers = {}
angle = 0
radius = 1.5
angularSpeed = 80

boxCollider = {isTrigger = true, center = {x = 0.01466, y = 0.01759207}, size = {x = 0.36885, y = 0.8966106}}

function OnEquipped()

	for i = 1, 3 do		
		daggers[i] = AddChild()
		SetComponent(daggers[i], "SpriteRenderer", {sprite = 0})
		SetComponent(daggers[i], "BoxCollider", boxCollider)
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

	SetRotation(daggers[1], angle + 180)
	SetRotation(daggers[2], angle + 180 + 120)
	SetRotation(daggers[3], angle + 180 + 240)

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

function OnEnemyHitStart(enemy)

	

end

function OnEnemyHitStay(enemy)

	-- 70 damage/second = 21 damage in 1/3 seconds = 1/3 seconds to kill one Squirt (Just as an example, not taking extra damage into account here)
	Damage(enemy, 70 * GetDT())

end


function OnEnemyHitExit()


end

return
{
	OnEquipped = OnEquipped,
	Update = Update,
	OnEnemyHitStart = OnEnemyHitStart,
	OnEnemyHitStay = OnEnemyHitStay,
	OnEnemyHitExit = OnEnemyHitExit,
}