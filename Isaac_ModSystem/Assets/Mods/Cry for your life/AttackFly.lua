extraTextures = { "monster_010_fly.png" }

--Init all the animation rects
red1 = {x = 71, y = 204, w = 19, h = 10}
black2 = {x = 41, y = 236, w = 13, h = 15}

animSpeed = 3 --sprites per second
flySpeed = 4

function Awake()

	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = red1})
	SetComponent(This(), "CapsuleCollider", {isTrigger = false, center = {x = 0, y = 0}, size = {x = 0.6333333, y = 0.3333333}, direction = "Horizontal"})

	Wait(1 / animSpeed, function() Red1() end)
end

function Red1()

	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = red1})
	Wait(1 / animSpeed, function() Black2() end)
end

function Black2()

	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = black2})
	Wait(1 / animSpeed, function() Red1() end)
end

function Update()

	local IsaacPos = GetPosition(0)
	local thisPos = GetPosition(This())

	local dir = {}
	dir.x = IsaacPos.x - thisPos.x
	dir.y = IsaacPos.y - thisPos.y
	dir = Normalize(dir)

	local dt = GetDT()

	local newPosition = {}
	newPosition.x = thisPos.x + dir.x * flySpeed * dt
	newPosition.y = thisPos.y + dir.y * flySpeed * dt

	SetPosition(This(), newPosition)
end

function OnEnemyDie()
	Notify("OnMonsterDied")
end

function Normalize(vector)

	local magnitude = math.sqrt(math.pow(vector.x, 2) + math.pow(vector.y, 2))

	local ret = {}
	ret.x = vector.x / magnitude
	ret.y = vector.y / magnitude

	return ret
end

return 
{
	Awake = Awake,
	Update = Update,
	OnEnemyDie = OnEnemyDie,
}

--red2 = {x = 41, y = 204, w = 13, h = 15}
--black1 = {x = 7, y = 236, w = 19, h = 10}

--function Red2()

--	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = red2})
--	Wait(1 / animSpeed, function() Black1() end)

--end

--function Black1()
	
--	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = black1})
--	Wait(1 / animSpeed, function() Black2() end)

--end