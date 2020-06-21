extraTextures = { "monster_001_pooter.png", "tears.png" }
audioClips = {"insect swarm.wav", "tear block.wav", "tear fire 5.wav"}
difficulty = 0.5

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

function OnBulletCollision(bullet, collided)
	if collided == 0 then 
		--Inflict damage to Isaac
		stats = GetStats()
		Damage(0, stats.damage)
	end

	for i = #bullets, 1, -1 do
		if bullets[i].bulletID == bullet then
			
			PlayFX(bullets[i].bulletID, 2)
			UnSubscribeEvent(bullets[i].event)
			bullets[i].update = false
			Wait(0.15, function()						
						DeleteChild(bullets[i].bulletID)	
						table.remove(bullets, i)
					  end)
			break
		end
	end
end

flyIndex = 1
flyTimer = 0
wanderDestination = nil

function FlyAnimation()
	flyTimer = flyTimer + GetDT()
	if flyTimer > 0.02 then
		flyIndex = flyIndex + 1
		if flyIndex > 2 then flyIndex = 1 end
		SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = fly[flyIndex]})
		flyTimer = 0
	end
end

attackIndex = 1
attackTimer = 0
attackRadius = 5

bulletLifeTime = 6
bullets = {}

function AttackAnimation()
	local pooterPos = GetPosition(This())
	local IsaacPos = GetPosition(0)

	--Flip horizontally to look at Isaac
	if pooterPos.x <= IsaacPos.x then
		SetScale(This(), {x = 1, y = 1})
	else
		SetScale(This(), {x = -1, y = 1})
	end

	attackTimer = attackTimer + GetDT()
	if attackTimer > 0.07 then
		attackIndex = attackIndex + 1
		if attackIndex > 14 then attackIndex = 1 end
		SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = attack[attackIndex]})
		attackTimer = 0

		--Synchronize the attack with the exact frame of the animation
		if attackIndex == 7 then 
			Attack()
		end
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
	--Reset the attack animation
	attackTimer = 0
	attackIndex = 1
	
	--Calculate the direction in local space
	local localPosition = GetLocalPosition(This())
	local direction = {}
	direction.x = wanderDestination.x - localPosition.x
	direction.y = wanderDestination.y - localPosition.y
	direction = Normalize(direction)
	
	local dt = GetDT()

	--Calculate the new local position using the speed stats
	local stats = GetStats()
	local newLocalPosition = {}
	newLocalPosition.x = localPosition.x + direction.x * dt * stats.speed * stats.speedFactor
	newLocalPosition.y = localPosition.y + direction.y * dt * stats.speed * stats.speedFactor

	--Clamp the position to the destination if they are too close after moving
	if math.abs(newLocalPosition.x - wanderDestination.x) < dt * stats.speed * stats.speedFactor then newLocalPosition.x = wanderDestination.x end
	if math.abs(newLocalPosition.y - wanderDestination.y) < dt * stats.speed * stats.speedFactor then newLocalPosition.y = wanderDestination.y end

	SetLocalPosition(This(), newLocalPosition)

	--If arrived to the destination, set another one
	if newLocalPosition.x == wanderDestination.x and newLocalPosition.y == wanderDestination.y then SetRandomWanderDestination() end

	--Flip horizontally to look at destination
	if newLocalPosition.x <= wanderDestination.x then
		SetScale(This(), {x = 1, y = 1})
	else
		SetScale(This(), {x = -1, y = 1})
	end
end

function Attack()
	--Instantiate a new monster tear and setup it
	local bullet = AddChild()
	SetComponent(bullet, "SpriteRenderer", {sprite = 2, rect = {x = 224, y = 32, w = 32, h = 32}})
	SetComponent(bullet, "Rigidbody", {})
	SetComponent(bullet, "CircleCollider", {radius = 0.19, center = {x = 0.016, y = 0.016}, isTrigger = true})
	SetLayer(bullet, "MonsterProjectile")
	SetParent(bullet, nil)
	event = SubscribeEvent(bullet, "OnTriggerEnter", OnBulletCollision)

	--Calculate its movement direction
	local bulletPos = GetPosition(bullet)
	local IsaacPos = GetPosition(0)

	local direction = {}
	direction.x = IsaacPos.x - bulletPos.x
	direction.y = IsaacPos.y - bulletPos.y
	direction = Normalize(direction)

	--Store it in the bullets table
	table.insert(bullets, {bulletID = bullet, time = 0, direction = direction, event = event, update = true})

	PlayFX(bullet, 3)
end

function UpdateBullets()
	local dt = GetDT()

	--Iterate the bullets backwards due to the deletion issues while going through the table
	for i = #bullets, 1, -1 do
		local bullet = bullets[i]

		--Calculate the new position
		local bulletPos = GetPosition(bullet.bulletID)
		local newPosition = {}
		newPosition.x = bulletPos.x + bullet.direction.x * dt * 3
		newPosition.y = bulletPos.y + bullet.direction.y * dt * 3

		if bullets[i].update then
			SetPosition(bullet.bulletID, newPosition)
		end

		--Destroy the bullet depending on its life time
		bullets[i].time = bullets[i].time + dt
		if bullets[i].time > bulletLifeTime then DeleteChild(bullet.bulletID) table.remove(bullets, i) end	
	end
end

function Awake()
	SetComponent(This(), "CapsuleCollider", {isTrigger = false, center = {x = 0, y = 0}, size = {x = 0.6333333, y = 0.3333333}, direction = "Horizontal"})
	SetStats({hp = 40, maxHP = 40, damage = 1, speed = 2})
	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = fly[flyIndex]})
	SetRandomWanderDestination()
	
	SetComponent(This(), "AudioSource", {volume = 0.3, loop = true, clip = 1})
	PlayFX(This(), 1)
end

function Update()
	if not enabled then return end
	
	UpdateBullets()

	local IsaacPos = GetPosition(0)
	local thisPos = GetPosition(This())

	local distance = {}
	distance.x = IsaacPos.x - thisPos.x
	distance.y = IsaacPos.y - thisPos.y
	distance = math.sqrt(math.pow(distance.x, 2) + math.pow(distance.y, 2))

	if distance <= attackRadius then
		AttackAnimation()
	else
		FlyAnimation()
		Wander()
	end
end

function OnEnemyDie()
	--Disable collider and logic
	enabled = false
	SetComponent(This(), "CapsuleCollider", {enabled = false})

	--Delete all the instantiated bullets
	for i = #bullets, 1, -1 do
		DeleteChild(bullets[i].bulletID) table.remove(bullets, i)
	end

	--Notify the room one enemy has been defeated
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