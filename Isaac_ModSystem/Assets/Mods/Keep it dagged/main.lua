sprite = "collectibles_172_sacrificialdagger.png"

passiveItems = 
{
	"SacrificialDagger.lua"
}

function Start()
	print("Mod Start")

	prevDamage = GetDamage()

	AddDamage(0.1)

	AddFactorDamage(1)

	print(string.format("Previous damage: %.2f. Current damage: %.2f", prevDamage, GetDamage()))

end

return
{
	Start = Start
}