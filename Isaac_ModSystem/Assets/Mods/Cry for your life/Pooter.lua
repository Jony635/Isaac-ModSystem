extraTextures = { "monster_001_pooter.png" }

--Init all the animation rects
fly = 
{
	{x = 0 , y = 96-0, w = 32, h = 32},
	{x = 32, y = 96-0, w = 32, h = 32},
}

attack = 
{
	{	x =	64, y = 96-0 , w = 32, h = 32 },
	{	x =	96, y = 96-0 , w = 32, h = 32 },
	{	x =	0 , y = 96-32, w = 32, h = 32 },
	{	x =	32, y = 96-32, w = 32, h = 32 },
	{	x =	64, y = 96-32, w = 32, h = 32 },
	{	x =	96, y = 96-32, w = 32, h = 32 },
	{	x =	0 , y = 96-64, w = 32, h = 32 },
	{	x =	32, y = 96-64, w = 32, h = 32 },
	{	x =	64, y = 96-64, w = 32, h = 32 },
	{	x =	96, y = 96-64, w = 32, h = 32 },
	{	x =	0 , y = 96-96, w = 32, h = 32 },
	{	x =	32, y = 96-96, w = 32, h = 32 },
	{	x =	64, y = 96-96, w = 32, h = 32 },
	{	x =	96, y = 96-96, w = 32, h = 32 },
}

enabled = true

flyIndex = 1
flyTimer = 0
attacking = false

function FlyAnimation()
	if attacking then flyTimer = 0 flyIndex = 1 return end

	flyTimer = flyTimer + GetDT()
	if flyTimer > 0.2 then
		flyIndex = flyIndex + 1
		if flyIndex > 2 then flyIndex = 1 end
		SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = fly[flyIndex]})
		flyTimer = 0
	end
end

function Awake()
	SetComponent(This(), "CapsuleCollider", {isTrigger = false, center = {x = 0, y = 0}, size = {x = 0.6333333, y = 0.3333333}, direction = "Horizontal"})
	SetStats({hp = 40, maxHP = 40, damage = 1, speed = 4})
	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = fly[flyIndex]})
end

function Update()

	if not enabled then return end
	
	FlyAnimation()

	--TODO: FLIP HORIZONTAL WHEN NEEDED TO LOOK AT ISAAC
	--TODO: SHOOTING
	--math.random() number between 0 and 1 (To wander)

	local IsaacPos = GetPosition(0)
	local thisPos = GetPosition(This())

	local dir = {}
	dir.x = IsaacPos.x - thisPos.x
	dir.y = IsaacPos.y - thisPos.y
	dir = Normalize(dir)

	local dt = GetDT()

	local stats = GetStats()

	local newPosition = {}
	newPosition.x = thisPos.x + dir.x * stats.speed * stats.speedFactor * dt
	newPosition.y = thisPos.y + dir.y * stats.speed * stats.speedFactor * dt

	SetPosition(This(), newPosition)
end

function OnEnemyDie()
	
	local animationDelay = 0.05

	--Disable collider and logic
	enabled = false
	SetComponent(This(), "CapsuleCollider", {enabled = false})

	--Play the animation
	return

	--SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die1})
	--Wait(animationDelay, 
	--function()
	--	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die2})
	--	Wait(animationDelay, 
	--	function()
	--		SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die3})
	--		Wait(animationDelay, 
	--		function()
	--			SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die4})
	--			Wait(animationDelay, 
	--			function()
	--				SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die5})
	--				Wait(animationDelay, 
	--				function()
	--					SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die6})
	--					Wait(animationDelay, 
	--					function()
	--						SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die7})
	--						Wait(animationDelay, 
	--						function() 
	--							SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die8})
	--							Wait(animationDelay, 
	--							function()
	--								SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die9})
	--								Wait(animationDelay, 
	--								function() 
	--									SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die10})
	--									Wait(animationDelay, 
	--									function()
	--										SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die11})
	--										Wait(animationDelay, 
	--										function()
	--											--Notify the current room
	--											Notify("OnMonsterDied")
	--										end)										
	--									end)								
	--								end)
	--							end)
	--						end)
	--					end)
	--				end)
	--			end)
	--		end)
	--	end)
	--end)
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