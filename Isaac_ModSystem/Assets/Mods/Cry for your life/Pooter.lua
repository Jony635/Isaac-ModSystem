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
wanderDestination = nil
attackRadius = 5

function FlyAnimation()
	if attacking then flyTimer = 0 flyIndex = 1 return end

	flyTimer = flyTimer + GetDT()
	if flyTimer > 0.02 then
		flyIndex = flyIndex + 1
		if flyIndex > 2 then flyIndex = 1 end
		SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = fly[flyIndex]})
		flyTimer = 0
	end
end

function SetRandomWanderDestination()
	-- Get a Vector2 whose members are between 0 and 1 (both inclusive)
	local rand = {x = math.random(), y = math.random()}

	--Convert this number into a [-1, 1] range
	rand.x = rand.x * 2 - 1
	rand.y = rand.y * 2 - 1
	
	--Get the real localPosition to set the destination
	rand.x = rand.x * 5
	rand.y = rand.y * 2.5
	
	--Assign the new destination to wanderDestination global variable
	wanderDestination = rand
end

function Wander()
	local stats = GetStats()

	if wanderDestination == nil then
		SetRandomWanderDestination()
	end

	local localPosition = GetLocalPosition(This())
	
	local direction = {}
	direction.x = wanderDestination.x - localPosition.x
	direction.y = wanderDestination.y - localPosition.y
	direction = Normalize(direction)
	
	local dt = GetDT()

	local newLocalPosition = {}
	newLocalPosition.x = localPosition.x + direction.x * dt * stats.speed * stats.speedFactor
	newLocalPosition.y = localPosition.y + direction.y * dt * stats.speed * stats.speedFactor

	if math.abs(newLocalPosition.x - wanderDestination.x) < dt * stats.speed * stats.speedFactor then newLocalPosition.x = wanderDestination.x end
	if math.abs(newLocalPosition.y - wanderDestination.y) < dt * stats.speed * stats.speedFactor then newLocalPosition.y = wanderDestination.y end

	SetLocalPosition(This(), newLocalPosition)

	if newLocalPosition.x == wanderDestination.x and newLocalPosition.y == wanderDestination.y then SetRandomWanderDestination() end

	--Flip horizontally to look at destination
	if newLocalPosition.x <= wanderDestination.x then
		SetScale(This(), {x = 1, y = 1})
	else
		SetScale(This(), {x = -1, y = 1})
	end

end

function Attack()


end

function Awake()
	SetComponent(This(), "CapsuleCollider", {isTrigger = false, center = {x = 0, y = 0}, size = {x = 0.6333333, y = 0.3333333}, direction = "Horizontal"})
	SetStats({hp = 40, maxHP = 40, damage = 1, speed = 2})
	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = fly[flyIndex]})
end

function Update()

	if not enabled then return end
	
	local IsaacPos = GetPosition(0)
	local thisPos = GetPosition(This())

	local distance = {}
	distance.x = IsaacPos.x - thisPos.x
	distance.y = IsaacPos.y - thisPos.y
	distance = math.sqrt(math.pow(distance.x, 2) + math.pow(distance.y, 2))

	if distance <= attackRadius then
		Attack()
	else
		FlyAnimation()
		Wander()
	end

end

function OnEnemyDie()
	
	local animationDelay = 0.05

	--Disable collider and logic
	enabled = false
	SetComponent(This(), "CapsuleCollider", {enabled = false})

	Notify("OnMonsterDied")

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