using UnityEngine;
using System.Collections;

public static class TextLiterals {

	/**********SHIP NAMES**********/
	public const string SHIP_NAME_LANCELOT = "Lancelot";
	public const string SHIP_NAME_NOSEFERATU = "Noseferatu";
	public const string SHIP_NAME_TEST_SUBJECT_P41N = "Test Subject #P-41N";
	public const string SHIP_NAME_JUNK_DRIVER = "Junk Driver";
	public const string SHIP_NAME_REDEYE = "Red-Eye";

	/**********SHIP DESCRIPTIONS**********/
	public const string SHIP_DESC_LANCELOT = "Lancelot is considered by many to be the deadliest warship in the Organization. However, its bright and lustrous exterior hides a dark past of corruption and misery. No general has been able to pilot the warship for more than a year before disappearing under mysterious circumstances. Despite its haunting reputation, few can resist the temptation of its incredibly versatile arsenal.";
	public const string SHIP_DESC_NOSEFERATU = "Although no one knows who, or what, pilots the pirate ship Noseferatu, it's technological capabilities are legendary among the Galaxy's traders. The ship is equipped with nanobot technology which can salvage the  wreckage of its victims to repair any damage sustained in a fight.";
	public const string SHIP_DESC_TEST_SUBJECT_P41N = "P-41N is the first artificial intelligence capable of experiencing pain. In its infancy, scientists subjected it to excruciating torture while monitoring its responses. However, due to faulty wiring, rather than developing an aversion to pain, P-41N seemed to grow fond of it, even crave it. Now the more it hurts, the stronger it becomes...";
	public const string SHIP_DESC_JUNK_DRIVER = "In the midst of a decade-long revolution, rebels constructed the Junk Driver out of a collection of spare parts and debris gathered in the aftermath of countless battles against the Organization. This warship was built to last, and stands as a lasting symbol of the Rebellion. It was not designed to annihilate its opponents, but rather outlive them. ";
	public const string SHIP_DESC_REDEYE = "Red-Eye is the pilot you contact when you want something erased from existence. As the Galaxy's most notorious assassin, she prefers to eliminate targets from a distance. Her arsenal consists entirely of weapons designed to destroy quickly and cleanly, believing that defensive fallbacks limit her capacity for killing. Many learn the hard way that her allegiance constantly shifts to whoever is offering the most money...";

	/**********MISC STATS**********/
	public const string MISC_STAT_LANCELOT = "Reliability";
	public const string MISC_STAT_NOSEFERATU = "Life Steal";
	public const string MISC_STAT_TEST_SUBJECT_P41N = "Tenacity";
	public const string MISC_STAT_JUNK_DRIVER = "Armor";
	public const string MISC_STAT_REDEYE = "Precision";

	/**********SHIP ABILITY NAMES**********/

	/*****LANCELOT*****/
	public const string ABILITY_NAME_LANCELOT_0 = "Fountain";
	public const string ABILITY_NAME_LANCELOT_1 = "Helicoid";
	public const string ABILITY_NAME_LANCELOT_2 = "X-Beam";
	public const string ABILITY_NAME_LANCELOT_3 = "Reflector";

	/*****NOSEFERATU*****/
	public const string ABILITY_NAME_NOSEFERATU_0 = "Skimmers";
	public const string ABILITY_NAME_NOSEFERATU_1 = "The Grinder";
	public const string ABILITY_NAME_NOSEFERATU_2 = "Leech Curse";
	public const string ABILITY_NAME_NOSEFERATU_3 = "Blood Rite";

	/*****TEST SUBJECT #P-41N*****/
	public const string ABILITY_NAME_TEST_SUBJECT_P41N_0 = "DNA Scrambler";
	public const string ABILITY_NAME_TEST_SUBJECT_P41N_1 = "Experiment-04";
	public const string ABILITY_NAME_TEST_SUBJECT_P41N_2 = "Overload";
	public const string ABILITY_NAME_TEST_SUBJECT_P41N_3 = "Torment Field";

	/*****JUNK DRIVER*****/
	public const string ABILITY_NAME_JUNK_DRIVER_0 = "Burst Battery";
	public const string ABILITY_NAME_JUNK_DRIVER_1 = "The Sprinkler";
	public const string ABILITY_NAME_JUNK_DRIVER_2 = "Cluster Mines";
	public const string ABILITY_NAME_JUNK_DRIVER_3 = "Gravity Corrupter";

	/*****RED-EYE*****/
	public const string ABILITY_NAME_REDEYE_0 = "The Beehive";
	public const string ABILITY_NAME_REDEYE_1 = "Blue Lotus";
	public const string ABILITY_NAME_REDEYE_2 = "Dual Lasers";
	public const string ABILITY_NAME_REDEYE_3 = "Precision Railgun";

	/**********SHIP ABILITY DESCRIPTIONS**********/
	
	/*****LANCELOT*****/
	public const string ABILITY_DESC_LANCELOT_0 = "Fires a semi-random spread of bullets directed at the enemy.";
	public const string ABILITY_DESC_LANCELOT_1 = "Emanates several bullets in a spiral pattern that changes directions multiple times.";
	public const string ABILITY_DESC_LANCELOT_2 = "Fires a massive beam attack that spans both vertically and horizontally, dealing damage and severely slowing any enemy within it. Useful for zoning off opponents or thwarting defensive countermeasures.";
	public const string ABILITY_DESC_LANCELOT_3 = "Creates a small dimensional field that converts enemy bullets and fires them back at the assailant.";

	/*****NOSEFERATU*****/
	public const string ABILITY_DESC_NOSEFERATU_0 = "Shoots multiple high-density strings of bullets with slight homing capabilities.";
	public const string ABILITY_DESC_NOSEFERATU_1 = "Fires multiple waves of bullets rotating in alternating directions.";
	public const string ABILITY_DESC_NOSEFERATU_2 = "Creates an energy field that saps life from any opponents within its radius, healing the user.";
	public const string ABILITY_DESC_NOSEFERATU_3 = "Generates a shield that causes enemy bullets to heal the user rather than inflict damage.";

	/*****TEST SUBJECT #P-41N*****/
	public const string ABILITY_DESC_TEST_SUBJECT_P41N_0 = "Generates a wave-like attack that has incredibly high bullet density. Despite its small size, it can be devestating if it hits. \n\n<color=red> Below 50% health:</color> Increased bullet speed and doubled wave width.";
	public const string ABILITY_DESC_TEST_SUBJECT_P41N_1 = "Emanates bullets in a 5-point star formation. Useful for locking opponents into an area. \n\n<color=red> Below 50% health:</color> Fires in an 8-point star formation.";
	public const string ABILITY_DESC_TEST_SUBJECT_P41N_2 = "Overloads the bomb's detonation trigger, creating a large explosion that inflicts massive damage close to its epicenter. \n\n<color=red> Below 50% health:</color> Doubled explosion radius and increased damage.";
    public const string ABILITY_DESC_TEST_SUBJECT_P41N_3 = "Absorbs and stores enemy bullets, then fires them back at their original owner. \n\n<color=red> Below 50% health:</color> Increased bullet capacity and firing speed.";

	/*****JUNK DRIVER*****/
	public const string ABILITY_DESC_JUNK_DRIVER_0 = "Fires a cone of bullets directed at the opponent. More effective at close range.";
	public const string ABILITY_DESC_JUNK_DRIVER_1 = "Sprays several bullets in the general direction of the opponent. Difficult to dodge, but has low damage output on average. Effective for picking off small bits of health at a time.";
	public const string ABILITY_DESC_JUNK_DRIVER_2 = "Spreads a field of mines that detonate in a chain explosion if triggered.";
	public const string ABILITY_DESC_JUNK_DRIVER_3 = "Sucks in any opponent bullets caught in a nearby vicinity, and explodes those bullets in all directions after a short time. Also deals direct damage to enemies caught in the center.";

	/*****RED-EYE*****/
	public const string ABILITY_DESC_REDEYE_0 = "Creates 3 groups of bullets that home in on the enemy's position. Can be difficult to dodge.";
	public const string ABILITY_DESC_REDEYE_1 = "Fires off several bullets that blossom into a deadly flower-shape.";
	public const string ABILITY_DESC_REDEYE_2 = "While held down, fires two small beams from each of the ship's wings. Enemies caught in the beams are damaged over time and slowed. This powerful attack taxes the ship engines, slowing Red-Eye. The beams lose their staying power the longer they persist.";
	public const string ABILITY_DESC_REDEYE_3 = "While held down, power is drawn from the engines and is routed to Red-Eye's railgun, slowing overall ship movement. Releasing when fully charged will fire an incredibly quick, precise, and powerful shot that inflicts massive damage. \n\n Releasing before the charge is finished will cancel the attack.";

	/*****MISC TITLES*****/
	public const string MID_ROUND_VICTORY_SCREEN = "(Press start to begin the next round)";
	public const string FINAL_VICTORY_SCREEN = "(Press start to return to the title screen)";
}
