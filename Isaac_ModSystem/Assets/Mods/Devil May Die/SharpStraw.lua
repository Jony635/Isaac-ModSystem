sprite = "collectibles_507_sharpstraw.png"

numCharges = 1
rechargeTime = 8
timer = 0.0

maxHealthPercentDamage = 0.1

function Update()

	if GetCurrentCharges() == 0 then

		if timer >= 0.0 then

			timer = timer - GetDT()
			SetActivePercent(1 - (timer/rechargeTime) )

		else
			timer = rechargeTime

		end

	end

end

function OnUsed()
	
	local amount, enemies = GetRoomEnemies()

	if amount > 0 then

		for i, enemy in ipairs(enemies) do

			Damage(enemy, GetDamage() + maxHealthPercentDamage * GetMaxHealth(enemy))

		end

	end

	timer = rechargeTime
	SetActivePercent(1 - (timer/rechargeTime) )

end

return
{
	Update = Update,
	OnUsed = OnUsed,
}
