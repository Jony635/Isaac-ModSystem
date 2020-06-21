sprite = "collectibles_093_thegamekid.png"
numCharges = 6

active = false
activeDuration = 6
activeTimer = activeDuration

damageSecond = 40

function OnUsed()

	active = true

	SetInvincible(true)

	Wait(activeDuration, function() active = false damageTimer = damageCooldown SetInvincible(false) activeTimer = activeDuration end)

end

function Update()
	if active then
		activeTimer = activeTimer - GetDT()
		SetMusic({pitch = activeTimer / activeDuration + 1})
	end
end

function OnCharacterCollidedWithMonster(enemy)

	if active then
		Damage(enemy, damageSecond * GetDT())
	end

end

function OnCharacterCollidingWithMonster(enemy)

	if active then
		Damage(enemy, damageSecond * GetDT())
	end

end

return
{
	OnUsed = OnUsed,
	OnCharacterCollidedWithMonster = OnCharacterCollidedWithMonster,
	OnCharacterCollidingWithMonster = OnCharacterCollidingWithMonster,
	Update = Update,
}