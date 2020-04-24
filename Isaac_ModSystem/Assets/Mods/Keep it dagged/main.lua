mod = require "ModSystem"

sprite = "collectibles_172_sacrificialdagger.png"

passiveItems = 
{
	"SacrificialDagger.lua"
}

function Start()
	print("Mod Start")

	prevDamage = mod.GetDamage()

	mod.AddDamage(0.1)

	mod.AddFactorDamage(1)

	print(string.format("Previous damage: %.2f. Current damage: %.2f", prevDamage, mod.GetDamage()))

end

return
{
	Start = Start
}