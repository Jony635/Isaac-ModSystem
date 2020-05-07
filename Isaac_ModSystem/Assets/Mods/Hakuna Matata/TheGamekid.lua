sprite = "collectibles_093_thegamekid.png"
numCharges = 6

active = false
activeDuration = 6
hitRatio = 1

damageCooldown = 1
damageTimer = damageCooldown
flexTime = 0.2

function OnUsed()

	active = true

	SetInvincible(true)

	Wait(activeDuration, function() active = false damageTimer = damageCooldown SetInvincible(false) end)

end

function OnCharacterCollidedWithMonster(enemy)

	if active and damageTimer <= flexTime then
		print("damage")
		Damage(enemy, 5)
	end
end

function OnCharacterCollidingWithMonster(enemy)

	if active and damageTimer <= flexTime then
		print("damage")
		Damage(enemy, 5)
	end

end

function Update()

	if active then

		damageTimer = damageTimer - GetDT()
		if damageTimer <= 0 then
			damageTimer = damageCooldown
		end

	end

end

return
{
	OnUsed = OnUsed,
	OnCharacterCollidedWithMonster = OnCharacterCollidedWithMonster,
	OnCharacterCollidingWithMonster = OnCharacterCollidingWithMonster,
	Update = Update,
}