extraTextures = { "monster_010_fly.png" }
audioClips = {"insect swarm.wav"}
difficulty = 1

--Init all the animation rects
red1 = {x = 71, y = 204, w = 19, h = 10}
black2 = {x = 41, y = 236, w = 13, h = 15}

die1 = {x = 22, y = 155, w = 17, h = 14}
die2 = {x = 75, y = 147, w = 32, h = 27}
die3 = {x = 135, y = 144, w = 44, h = 33}
die4 = {x = 196, y = 136, w = 53, h = 44}
die5 = {x = 3, y = 71, w = 67, h = 45}
die6 = {x = 71, y = 68, w = 60, h = 47}
die7 = {x = 143, y = 66, w = 51, h = 47}
die8 = {x = 207, y = 68, w = 38, h = 45}
die9 = {x = 15, y = 15, w = 33, h = 34}
die10 = {x = 79, y = 16, w = 33, h = 33}
die11 = {x = 146, y = 16, w = 30, h = 34}

animSpeed = 3 --sprites per second

enabled = true

function Awake()

	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = red1})
	SetComponent(This(), "CapsuleCollider", {isTrigger = false, center = {x = 0, y = 0}, size = {x = 0.6333333, y = 0.3333333}, direction = "Horizontal"})

	SetStats({hp = 40, maxHP = 40, damage = 1, speed = 4})

	Wait(1 / animSpeed, function() Red1() end)

	SetComponent(This(), "AudioSource", {volume = 0.3, loop = true, clip = 1})
	PlayFX(This(), 1)
end

function Red1()
	if not enabled then return end
	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = red1})
	Wait(1 / animSpeed, function() Black2() end)
end

function Black2()
	if not enabled then return end
	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = black2})
	Wait(1 / animSpeed, function() Red1() end)
end

function Update()

	if not enabled then return end
	
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
	SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die1})
	Wait(animationDelay, 
	function()
		SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die2})
		Wait(animationDelay, 
		function()
			SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die3})
			Wait(animationDelay, 
			function()
				SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die4})
				Wait(animationDelay, 
				function()
					SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die5})
					Wait(animationDelay, 
					function()
						SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die6})
						Wait(animationDelay, 
						function()
							SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die7})
							Wait(animationDelay, 
							function() 
								SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die8})
								Wait(animationDelay, 
								function()
									SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die9})
									Wait(animationDelay, 
									function() 
										SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die10})
										Wait(animationDelay, 
										function()
											SetComponent(This(), "SpriteRenderer", {sprite = 1, rect = die11})
											Wait(animationDelay, 
											function()
												--Notify the current room
												Notify("OnMonsterDied")
											end)										
										end)								
									end)
								end)
							end)
						end)
					end)
				end)
			end)
		end)
	end)

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