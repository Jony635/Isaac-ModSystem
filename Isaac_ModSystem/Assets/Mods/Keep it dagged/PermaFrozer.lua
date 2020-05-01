sprite = "collectibles_110_momscontacts.png"
numCharges = 1

active = false
duration = 4

slowDuration = 1
slowPercent = 0.5

frozenEnemies = 
{
	Contains = function (self, enemy)

			local i = 1
			while self[i] ~= nil do
		
				if self[i].ref == enemy then
					return true
				end

				i = i + 1
			end

			return false

		end
}


function OnUsed()
	active = true;
	
	Wait(duration, function()		
		active = false
	end)
end

function OnMonsterHittedByTear(enemyRef)

	if active then

		if not frozenEnemies:Contains(enemyRef) then
			Slow(enemyRef, slowPercent)
			enemy = {}
			enemy.timer = slowDuration
			enemy.ref = enemyRef
			table.insert(frozenEnemies, enemy)
		else
			--REFRESH THE TIMER IF THE PLAYER KEEPS HITTING A SLOWED ENEMY
			local i = 1
			while frozenEnemies[i] ~= nil do
		
				if frozenEnemies[i].ref == enemyRef then
					frozenEnemies[i].timer = slowDuration
					break
				end

				i = i + 1
			end
		end
	end

	

end

function OnNewRoomEntered(alreadyDefeated)
	active = false;
end

function Update()

	local dt = GetDT()
	
	local i = 1
	while frozenEnemies[i] ~= nil do
		frozenEnemies[i].timer = frozenEnemies[i].timer - dt

		if frozenEnemies[i].timer <= 0 then
			ClearSlow(frozenEnemies[i].ref, slowPercent)
			table.remove(frozenEnemies, i)
			i = i - 1
		end
		
		i = i + 1
	end

end



return
{
	OnUsed = OnUsed,
	OnMonsterHittedByTear = OnMonsterHittedByTear,
	OnNewRoomEntered = OnNewRoomEntered,
	Update = Update,
}