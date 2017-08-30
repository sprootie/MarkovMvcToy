/*
 * Copyright (c) 2015 Globys, Inc., All Rights Reserved.
 */
namespace Markov.Data
{
  using System;
  using System.Linq;
  using System.Collections.Generic;

  /// <summary>
  /// Random Harry Potter title generator.  Wheee!
  /// </summary>
  public static class RandomTitle
  {
    private static readonly Random Rng = new Random();

    /// <summary>
    /// Types of nouns
    /// </summary>
    [Flags]
    enum NounType
    {
      /// <summary>
      /// Proper Name, eg Dumbledore
      /// </summary>
      Proper = 1,
      /// <summary>
      /// Non-specific person, eg King
      /// </summary>
      Person = 2,
      /// <summary>
      /// Location, eg Camelot
      /// </summary>
      Place = 4,
      /// <summary>
      /// Abstract noun, eg Darkness, Infinity
      /// </summary>
      DescriptiveThing = 8,
      /// <summary>
      /// Singular object, eg Sword
      /// </summary>
      SingleObject = 16,
      /// <summary>
      /// Plural or indistinct object, eg Swords, Knowledge
      /// </summary>
      Object = 32,
      /// <summary>
      /// Noun that can be used like an adjective, eg Iron, Crystal
      /// </summary>
      Attribute = 64,
      /// <summary>
      /// Non-specific group of individuals, eg Beasts
      /// </summary>
      People = 128
    }
    
    /// <summary>
    /// Prefix headers
    /// </summary>
    private static readonly List<string> Prefixes = new List<string> { "Harry Potter and the", "The Secret of the", "Attack of the", "Revenge of the", "Return of the", "War of the" };

    /// <summary>
    /// Potential targets for an "object of" preposition, eg "Dagger of"
    /// </summary>
    private static readonly NounType ObjectOfTarget =
      NounType.Attribute.Add(NounType.Object).Add(NounType.DescriptiveThing).Add(NounType.Place).Add(NounType.Proper).Add(NounType.People);

    /// <summary>
    /// Potential targets for a "person of" preposition, eg "King of"
    /// </summary>
    private static readonly NounType PersonOfTarget =
      NounType.DescriptiveThing.Add(NounType.Place).Add(NounType.Proper).Add(NounType.Object).Add(NounType.People);

    /// <summary>
    /// Potential targets for a "person" preposition, eg "King"
    /// </summary>
    private static readonly NounType PersonTarget =
      NounType.SingleObject.Add(NounType.Object);

    /// <summary>
    /// Potential targets for a "person's" preposition, eg "King's"
    /// </summary>
    private static readonly NounType PossessiveTarget =
      NounType.SingleObject.Add(NounType.Object).Add(NounType.Attribute).Add(NounType.DescriptiveThing).Add(NounType.Person);

    /// <summary>
    /// Potential targets for an "adjective" preposition, eg "Broken", "Glamourous"
    /// </summary>
    private static readonly NounType AdjectiveTarget =
      NounType.Attribute.Add(NounType.DescriptiveThing).Add(NounType.Object).Add(NounType.SingleObject).Add(NounType.Person).Add(NounType.People);


    /// <summary>
    /// List of prepositions and their targets
    /// </summary>
    private static readonly List<Tuple<string, NounType>> Prepositions = new List<Tuple<string, NounType>>
    {
      new Tuple<string, NounType>("Alchemical", AdjectiveTarget),
      new Tuple<string, NounType>("Alchemist", PersonTarget),
      new Tuple<string, NounType>("Alchemist of", PersonOfTarget),
      new Tuple<string, NounType>("Alchemist's", PossessiveTarget),
      new Tuple<string, NounType>("Amazing", AdjectiveTarget),
      new Tuple<string, NounType>("Amulet of", ObjectOfTarget),
      new Tuple<string, NounType>("Ancient", AdjectiveTarget),
      new Tuple<string, NounType>("Aquatic", AdjectiveTarget),
      new Tuple<string, NounType>("Arch-Druid", PersonTarget),
      new Tuple<string, NounType>("Arch-Druid of", PersonOfTarget),
      new Tuple<string, NounType>("Arch-Druid's", PossessiveTarget),
      new Tuple<string, NounType>("Army of ", ObjectOfTarget),
      new Tuple<string, NounType>("Axe of", ObjectOfTarget),
      new Tuple<string, NounType>("Bag of", ObjectOfTarget),
      new Tuple<string, NounType>("Barbarian", PersonTarget),
      new Tuple<string, NounType>("Barbarian of", PersonOfTarget),
      new Tuple<string, NounType>("Barbarian's", PossessiveTarget),
      new Tuple<string, NounType>("Battle of ", ObjectOfTarget),
      new Tuple<string, NounType>("Blade of", ObjectOfTarget),
      new Tuple<string, NounType>("Book of", ObjectOfTarget),
      new Tuple<string, NounType>("Bottle of", ObjectOfTarget),
      new Tuple<string, NounType>("Broken", AdjectiveTarget),
      new Tuple<string, NounType>("Bronze", AdjectiveTarget),
      new Tuple<string, NounType>("Cape of", ObjectOfTarget),
      new Tuple<string, NounType>("Castle of", ObjectOfTarget),
      new Tuple<string, NounType>("Cave of", ObjectOfTarget),
      new Tuple<string, NounType>("Celestial", AdjectiveTarget),
      new Tuple<string, NounType>("Centaur", PersonTarget),
      new Tuple<string, NounType>("Centaur of", PersonOfTarget),
      new Tuple<string, NounType>("Centaur's", PossessiveTarget),
      new Tuple<string, NounType>("Chain of", ObjectOfTarget),
      new Tuple<string, NounType>("Chalice of", ObjectOfTarget),
      new Tuple<string, NounType>("Chamber of", ObjectOfTarget),
      new Tuple<string, NounType>("Chest of", ObjectOfTarget),
      new Tuple<string, NounType>("Cold", AdjectiveTarget),
      new Tuple<string, NounType>("Confused", AdjectiveTarget),
      new Tuple<string, NounType>("Conjurer", PersonTarget),
      new Tuple<string, NounType>("Conjurer of", PersonOfTarget),
      new Tuple<string, NounType>("Conjurer's", PossessiveTarget),
      new Tuple<string, NounType>("Crazed", AdjectiveTarget),
      new Tuple<string, NounType>("Crossbow of", ObjectOfTarget),
      new Tuple<string, NounType>("Crown of", ObjectOfTarget),
      new Tuple<string, NounType>("Curse of", ObjectOfTarget),
      new Tuple<string, NounType>("Dagger of", ObjectOfTarget),
      new Tuple<string, NounType>("Dark", AdjectiveTarget),
      new Tuple<string, NounType>("Darkness", PersonTarget),
      new Tuple<string, NounType>("Darkness of", PersonOfTarget),
      new Tuple<string, NounType>("Darkness's", PossessiveTarget),
      new Tuple<string, NounType>("Death", PersonTarget),
      new Tuple<string, NounType>("Death of", PersonOfTarget),
      new Tuple<string, NounType>("Death's", PossessiveTarget),
      new Tuple<string, NounType>("Deathly", AdjectiveTarget),
      new Tuple<string, NounType>("Defeat of", ObjectOfTarget),
      new Tuple<string, NounType>("Defender", PersonTarget),
      new Tuple<string, NounType>("Defender of", PersonOfTarget),
      new Tuple<string, NounType>("Defender's", PossessiveTarget),
      new Tuple<string, NounType>("Destroyer", PersonTarget),
      new Tuple<string, NounType>("Destroyer of", PersonOfTarget),
      new Tuple<string, NounType>("Destroyer's", PossessiveTarget),
      new Tuple<string, NounType>("Diviner", PersonTarget),
      new Tuple<string, NounType>("Diviner of", PersonOfTarget),
      new Tuple<string, NounType>("Diviner's", PossessiveTarget),
      new Tuple<string, NounType>("Doom of", ObjectOfTarget),
      new Tuple<string, NounType>("Dragon", PersonTarget),
      new Tuple<string, NounType>("Dragon of", PersonOfTarget),
      new Tuple<string, NounType>("Dragon's", PossessiveTarget),
      new Tuple<string, NounType>("Dungeon of", ObjectOfTarget),
      new Tuple<string, NounType>("Elf", PersonTarget),
      new Tuple<string, NounType>("Elf of", PersonOfTarget),
      new Tuple<string, NounType>("Elf's", PersonTarget),
      new Tuple<string, NounType>("Ethereal", AdjectiveTarget),
      new Tuple<string, NounType>("Evil", PersonTarget),
      new Tuple<string, NounType>("Exorcist", PersonTarget),
      new Tuple<string, NounType>("Exorcist of", PersonOfTarget),
      new Tuple<string, NounType>("Exorcist's", PossessiveTarget),
      new Tuple<string, NounType>("Fall of ", ObjectOfTarget),
      new Tuple<string, NounType>("Famous", AdjectiveTarget),
      new Tuple<string, NounType>("Fist of", ObjectOfTarget),
      new Tuple<string, NounType>("Final", AdjectiveTarget),
      new Tuple<string, NounType>("First", AdjectiveTarget),
      new Tuple<string, NounType>("Flaming", AdjectiveTarget),
      new Tuple<string, NounType>("Flawed", AdjectiveTarget),
      new Tuple<string, NounType>("Forbidden", AdjectiveTarget),
      new Tuple<string, NounType>("Fortress of", ObjectOfTarget),
      new Tuple<string, NounType>("Frozen", AdjectiveTarget),
      new Tuple<string, NounType>("Giant", AdjectiveTarget),
      new Tuple<string, NounType>("Giant of", PersonOfTarget),
      new Tuple<string, NounType>("Giant's", PossessiveTarget),
      new Tuple<string, NounType>("Ghoulish", PersonTarget),
      new Tuple<string, NounType>("Ghoul", PersonTarget),
      new Tuple<string, NounType>("Ghoul of", PersonOfTarget),
      new Tuple<string, NounType>("Ghoul's", PossessiveTarget),
      new Tuple<string, NounType>("Glamorous", AdjectiveTarget),
      new Tuple<string, NounType>("Glorious", AdjectiveTarget),
      new Tuple<string, NounType>("Glove of", ObjectOfTarget),
      new Tuple<string, NounType>("Goblet of", ObjectOfTarget),
      new Tuple<string, NounType>("Golden", AdjectiveTarget),
      new Tuple<string, NounType>("Grand", AdjectiveTarget),
      new Tuple<string, NounType>("Griffin", PersonTarget),
      new Tuple<string, NounType>("Griffin of", PersonOfTarget),
      new Tuple<string, NounType>("Griffin's", PossessiveTarget),
      new Tuple<string, NounType>("Half-Blood", PersonTarget),
      new Tuple<string, NounType>("Hammer of", ObjectOfTarget),
      new Tuple<string, NounType>("Haunted", AdjectiveTarget),
      new Tuple<string, NounType>("Heavenly", AdjectiveTarget),
      new Tuple<string, NounType>("Helm of", ObjectOfTarget),
      new Tuple<string, NounType>("Hidden", AdjectiveTarget),
      new Tuple<string, NounType>("Hero", PersonTarget),
      new Tuple<string, NounType>("Hero of", PersonOfTarget),
      new Tuple<string, NounType>("Hero's", PossessiveTarget),
      new Tuple<string, NounType>("Horn of", ObjectOfTarget),
      new Tuple<string, NounType>("Ice", PersonTarget),
      new Tuple<string, NounType>("Icy", AdjectiveTarget),
      new Tuple<string, NounType>("Immortal", AdjectiveTarget),
      new Tuple<string, NounType>("Insane", AdjectiveTarget),
      new Tuple<string, NounType>("Iron", AdjectiveTarget),
      new Tuple<string, NounType>("King", PersonTarget),
      new Tuple<string, NounType>("King of", PersonOfTarget),
      new Tuple<string, NounType>("King's", PossessiveTarget),
      new Tuple<string, NounType>("Knight", PersonTarget),
      new Tuple<string, NounType>("Knight of", PersonOfTarget),
      new Tuple<string, NounType>("Knight's", PossessiveTarget),
      new Tuple<string, NounType>("Labyrinth of", ObjectOfTarget),
      new Tuple<string, NounType>("Lake of", ObjectOfTarget),
      new Tuple<string, NounType>("Lantern of", ObjectOfTarget),
      new Tuple<string, NounType>("Last", AdjectiveTarget),
      new Tuple<string, NounType>("Leafy", AdjectiveTarget),
      new Tuple<string, NounType>("Legend of", ObjectOfTarget),
      new Tuple<string, NounType>("Light", AdjectiveTarget),
      new Tuple<string, NounType>("Light of", ObjectOfTarget),
      new Tuple<string, NounType>("Lightning", AdjectiveTarget),
      new Tuple<string, NounType>("Lightning of", ObjectOfTarget),
      new Tuple<string, NounType>("Lost", PersonTarget),
      new Tuple<string, NounType>("Lunar", AdjectiveTarget),
      new Tuple<string, NounType>("Magical", AdjectiveTarget),
      new Tuple<string, NounType>("Map of", ObjectOfTarget),
      new Tuple<string, NounType>("Marvelous", AdjectiveTarget),
      new Tuple<string, NounType>("Maze of", ObjectOfTarget),
      new Tuple<string, NounType>("Mines of", ObjectOfTarget),
      new Tuple<string, NounType>("Missing", AdjectiveTarget),
      new Tuple<string, NounType>("Monster", PersonTarget),
      new Tuple<string, NounType>("Monster of", PersonOfTarget),
      new Tuple<string, NounType>("Monster's", PossessiveTarget),
      new Tuple<string, NounType>("Mountain of", ObjectOfTarget),
      new Tuple<string, NounType>("Musical", AdjectiveTarget),
      new Tuple<string, NounType>("Mystical", AdjectiveTarget),
      new Tuple<string, NounType>("Myth of", ObjectOfTarget),
      new Tuple<string, NounType>("Order of", ObjectOfTarget),
      new Tuple<string, NounType>("Peasant", PersonTarget),
      new Tuple<string, NounType>("Peasant of", PersonOfTarget),
      new Tuple<string, NounType>("Peasant's", PossessiveTarget),
      new Tuple<string, NounType>("Pendant of", ObjectOfTarget),
      new Tuple<string, NounType>("Philosopher", PersonTarget),
      new Tuple<string, NounType>("Philosopher of", PersonOfTarget),
      new Tuple<string, NounType>("Philosopher's", PossessiveTarget),
      new Tuple<string, NounType>("Perfect", AdjectiveTarget),
      new Tuple<string, NounType>("Poisonous", AdjectiveTarget),
      new Tuple<string, NounType>("Pool of", ObjectOfTarget),
      new Tuple<string, NounType>("Prince", PersonTarget),
      new Tuple<string, NounType>("Prince of", PersonOfTarget),
      new Tuple<string, NounType>("Prince's", PossessiveTarget),
      new Tuple<string, NounType>("Prison of", ObjectOfTarget),
      new Tuple<string, NounType>("Prisoner", PersonTarget),
      new Tuple<string, NounType>("Prisoner of", PersonOfTarget),
      new Tuple<string, NounType>("Prisoner's", PossessiveTarget),
      new Tuple<string, NounType>("Puzzle of", ObjectOfTarget),
      new Tuple<string, NounType>("Reign of ", ObjectOfTarget),
      new Tuple<string, NounType>("Ressurected", AdjectiveTarget),
      new Tuple<string, NounType>("Ring of", ObjectOfTarget),
      new Tuple<string, NounType>("Rise of ", ObjectOfTarget),
      new Tuple<string, NounType>("Ritual of", ObjectOfTarget),
      new Tuple<string, NounType>("Rotten", AdjectiveTarget),
      new Tuple<string, NounType>("School of", ObjectOfTarget),
      new Tuple<string, NounType>("Scribe", PersonTarget),
      new Tuple<string, NounType>("Scribe of", PersonOfTarget),
      new Tuple<string, NounType>("Scribe's", PossessiveTarget),
      new Tuple<string, NounType>("Scroll of", ObjectOfTarget),
      new Tuple<string, NounType>("Sceptor of", ObjectOfTarget),
      new Tuple<string, NounType>("Secret of", ObjectOfTarget),
      new Tuple<string, NounType>("Sea of", ObjectOfTarget),
      new Tuple<string, NounType>("Serpent", PersonTarget),
      new Tuple<string, NounType>("Serpent of", PersonOfTarget),
      new Tuple<string, NounType>("Serpent's", PossessiveTarget),
      new Tuple<string, NounType>("Servant", PersonTarget),
      new Tuple<string, NounType>("Servant of", PersonOfTarget),
      new Tuple<string, NounType>("Servant's", PossessiveTarget),
      new Tuple<string, NounType>("Shadowy", AdjectiveTarget),
      new Tuple<string, NounType>("Shattered", AdjectiveTarget),
      new Tuple<string, NounType>("Shroud of", ObjectOfTarget),
      new Tuple<string, NounType>("Slayer", PersonTarget),
      new Tuple<string, NounType>("Slayer of", PersonOfTarget),
      new Tuple<string, NounType>("Slayer's", PossessiveTarget),
      new Tuple<string, NounType>("Sorcerer", PersonTarget),
      new Tuple<string, NounType>("Sorcerer of", PersonOfTarget),
      new Tuple<string, NounType>("Sorcerer's", PossessiveTarget),
      new Tuple<string, NounType>("Sorceress", PersonTarget),
      new Tuple<string, NounType>("Sorceress of", PersonOfTarget),
      new Tuple<string, NounType>("Sorceress'", PossessiveTarget),
      new Tuple<string, NounType>("Spellbook of", ObjectOfTarget),
      new Tuple<string, NounType>("Spear of", ObjectOfTarget),
      new Tuple<string, NounType>("Spirit", PersonTarget),
      new Tuple<string, NounType>("Spirit of", PersonOfTarget),
      new Tuple<string, NounType>("Spirit's", PossessiveTarget),
      new Tuple<string, NounType>("Staff of", ObjectOfTarget),
      new Tuple<string, NounType>("Star of", ObjectOfTarget),
      new Tuple<string, NounType>("Stone", AdjectiveTarget),
      new Tuple<string, NounType>("Stone of", ObjectOfTarget),
      new Tuple<string, NounType>("Stormy", AdjectiveTarget),
      new Tuple<string, NounType>("Storm of", ObjectOfTarget),
      new Tuple<string, NounType>("Sword of", ObjectOfTarget),
      new Tuple<string, NounType>("Teachor", PersonTarget),
      new Tuple<string, NounType>("Teachor of", PersonOfTarget),
      new Tuple<string, NounType>("Teachor's", PossessiveTarget),
      new Tuple<string, NounType>("Thief", PersonTarget),
      new Tuple<string, NounType>("Thief of", PersonOfTarget),
      new Tuple<string, NounType>("Thief's", PossessiveTarget),
      new Tuple<string, NounType>("Thunderous", AdjectiveTarget),
      new Tuple<string, NounType>("Time of", ObjectOfTarget),
      new Tuple<string, NounType>("Torch of", ObjectOfTarget),
      new Tuple<string, NounType>("Traitor", PersonTarget),
      new Tuple<string, NounType>("Traitor of", PersonOfTarget),
      new Tuple<string, NounType>("Traitor's", PossessiveTarget),
      new Tuple<string, NounType>("Triumph of", ObjectOfTarget),
      new Tuple<string, NounType>("Twisted", AdjectiveTarget),
      new Tuple<string, NounType>("Ultimate", AdjectiveTarget),
      new Tuple<string, NounType>("Undead", AdjectiveTarget),
      new Tuple<string, NounType>("Unending", AdjectiveTarget),
      new Tuple<string, NounType>("Unstoppable", AdjectiveTarget),
      new Tuple<string, NounType>("Unthinkable", AdjectiveTarget),
      new Tuple<string, NounType>("Unholy", AdjectiveTarget),
      new Tuple<string, NounType>("Unicorn", PersonTarget),
      new Tuple<string, NounType>("Unicorn of", PersonOfTarget),
      new Tuple<string, NounType>("Unicorn's", PossessiveTarget),
      new Tuple<string, NounType>("Vision of ", ObjectOfTarget),
      new Tuple<string, NounType>("Wand of", ObjectOfTarget),
      new Tuple<string, NounType>("War of ", ObjectOfTarget),
      new Tuple<string, NounType>("Warlock", PersonTarget),
      new Tuple<string, NounType>("Warlock of", PersonOfTarget),
      new Tuple<string, NounType>("Warlock's", PossessiveTarget),
      new Tuple<string, NounType>("Warrior", PersonTarget),
      new Tuple<string, NounType>("Warrior of", PersonOfTarget),
      new Tuple<string, NounType>("Warrior's", PossessiveTarget),
      new Tuple<string, NounType>("Weird", AdjectiveTarget),
      new Tuple<string, NounType>("Wicked", AdjectiveTarget),
      new Tuple<string, NounType>("Witch", PersonTarget),
      new Tuple<string, NounType>("Witch of", PersonOfTarget),
      new Tuple<string, NounType>("Witch's", PossessiveTarget),
      new Tuple<string, NounType>("Wonderous", AdjectiveTarget),
      new Tuple<string, NounType>("Wooden", AdjectiveTarget),
      new Tuple<string, NounType>("Zombie", PersonTarget),
      new Tuple<string, NounType>("Zombie of", PersonOfTarget),
      new Tuple<string, NounType>("Zombie's", PossessiveTarget),
    };

    /// <summary>
    /// List of nouns and their noun types
    /// </summary>
    private static readonly List<Tuple<string, NounType>> Nouns = new List<Tuple<string, NounType>>
    {
      new Tuple<string, NounType>("Acid", NounType.Attribute),
      new Tuple<string, NounType>("Agony", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Animal", NounType.Person),
      new Tuple<string, NounType>("Animals", NounType.People),
      new Tuple<string, NounType>("Annihilation", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Asgard", NounType.Place),
      new Tuple<string, NounType>("Avalon", NounType.Place),
      new Tuple<string, NounType>("Atlantis", NounType.Place),
      new Tuple<string, NounType>("Azakaban", NounType.Place),
      new Tuple<string, NounType>("Barbarian", NounType.Person),
      new Tuple<string, NounType>("Barbarians", NounType.People),
      new Tuple<string, NounType>("Beast", NounType.Person),
      new Tuple<string, NounType>("Beasts", NounType.People),
      new Tuple<string, NounType>("Blackness", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Blade", NounType.SingleObject),
      new Tuple<string, NounType>("Blades", NounType.Object),
      new Tuple<string, NounType>("Bone", NounType.Attribute),
      new Tuple<string, NounType>("Bones", NounType.Object),
      new Tuple<string, NounType>("Brimstone", NounType.Attribute),
      new Tuple<string, NounType>("Call", NounType.SingleObject),
      new Tuple<string, NounType>("Camelot", NounType.Place),
      new Tuple<string, NounType>("Centaur", NounType.Person),
      new Tuple<string, NounType>("Centaurs", NounType.People),
      new Tuple<string, NounType>("Chain", NounType.Attribute),
      new Tuple<string, NounType>("Chains", NounType.Object),
      new Tuple<string, NounType>("Chalice", NounType.SingleObject),
      new Tuple<string, NounType>("Cold", NounType.Attribute),
      new Tuple<string, NounType>("Confusion", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Control", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Crossbow", NounType.SingleObject),
      new Tuple<string, NounType>("Cudgel", NounType.SingleObject),
      new Tuple<string, NounType>("Curse", NounType.SingleObject),
      new Tuple<string, NounType>("Curses", NounType.Object),
      new Tuple<string, NounType>("Crystal", NounType.Attribute),
      new Tuple<string, NounType>("Crystals", NounType.Object),
      new Tuple<string, NounType>("Dagger", NounType.SingleObject),
      new Tuple<string, NounType>("Daggers", NounType.Object),
      new Tuple<string, NounType>("Darkness", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Death", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Death Eater", NounType.Person),
      new Tuple<string, NounType>("Death Eaters", NounType.People),
      new Tuple<string, NounType>("Demon", NounType.Person),
      new Tuple<string, NounType>("Demons", NounType.People),
      new Tuple<string, NounType>("Destroyer", NounType.Person),
      new Tuple<string, NounType>("Destroyers", NounType.People),
      new Tuple<string, NounType>("Devil", NounType.Person),
      new Tuple<string, NounType>("Devils", NounType.People),
      new Tuple<string, NounType>("Dreams", NounType.Object),
      new Tuple<string, NounType>("Dumbledore", NounType.Proper),
      new Tuple<string, NounType>("Dungeon", NounType.SingleObject),
      new Tuple<string, NounType>("Dungeons", NounType.Object),
      new Tuple<string, NounType>("Durmstrang", NounType.Proper),
      new Tuple<string, NounType>("Eden", NounType.Place),
      new Tuple<string, NounType>("El Dorado", NounType.Place),
      new Tuple<string, NounType>("Eternity", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Flame", NounType.Attribute),
      new Tuple<string, NounType>("Flames", NounType.Object),
      new Tuple<string, NounType>("Fire", NounType.Object),
      new Tuple<string, NounType>("Flesh", NounType.Attribute),
      new Tuple<string, NounType>("Forever", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Forgiveness", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Gate", NounType.SingleObject),
      new Tuple<string, NounType>("Ghoul", NounType.Person),
      new Tuple<string, NounType>("Ghouls", NounType.People),
      new Tuple<string, NounType>("Gloom", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Glove", NounType.SingleObject),
      new Tuple<string, NounType>("Gloves", NounType.Object),
      new Tuple<string, NounType>("Goblet", NounType.SingleObject),
      new Tuple<string, NounType>("Goblin", NounType.Person),
      new Tuple<string, NounType>("Goblins", NounType.People),
      new Tuple<string, NounType>("Gold", NounType.Attribute),
      new Tuple<string, NounType>("Greece", NounType.Place),
      new Tuple<string, NounType>("Gryffyndor", NounType.Proper),
      new Tuple<string, NounType>("Hallows", NounType.Object),
      new Tuple<string, NounType>("Hammer", NounType.SingleObject),
      new Tuple<string, NounType>("Heaven", NounType.Place),
      new Tuple<string, NounType>("Hero", NounType.Person),
      new Tuple<string, NounType>("Heroes", NounType.People),
      new Tuple<string, NounType>("Hogwarts", NounType.Place),
      new Tuple<string, NounType>("Horcrux", NounType.SingleObject),
      new Tuple<string, NounType>("Hufflepuff", NounType.Proper),
      new Tuple<string, NounType>("Ice", NounType.Attribute),
      new Tuple<string, NounType>("Imagination", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Immortality", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Infinity", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Insanity", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Iron", NounType.Attribute),
      new Tuple<string, NounType>("King", NounType.Person),
      new Tuple<string, NounType>("Kings", NounType.People),
      new Tuple<string, NounType>("Knowledge", NounType.Object),
      new Tuple<string, NounType>("Legend", NounType.SingleObject),
      new Tuple<string, NounType>("Legends", NounType.Object),
      new Tuple<string, NounType>("Lies", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Light", NounType.Attribute),
      new Tuple<string, NounType>("Limbo", NounType.Place),
      new Tuple<string, NounType>("London", NounType.Place),
      new Tuple<string, NounType>("Loss", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Luck", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Madness", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Mask", NounType.SingleObject),
      new Tuple<string, NounType>("Master", NounType.Person),
      new Tuple<string, NounType>("Masters", NounType.People),
      new Tuple<string, NounType>("Maze", NounType.SingleObject),
      new Tuple<string, NounType>("Mazes", NounType.Object),
      new Tuple<string, NounType>("Mentor", NounType.Person),
      new Tuple<string, NounType>("Mentors", NounType.People),
      new Tuple<string, NounType>("Merlin", NounType.Proper),
      new Tuple<string, NounType>("Ministry of Magic", NounType.Place),
      new Tuple<string, NounType>("Misery", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Monument", NounType.SingleObject),
      new Tuple<string, NounType>("Mystery", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Nightmares", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Nowhere", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Order", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Oblivion", NounType.Place),
      new Tuple<string, NounType>("Olympus", NounType.Place),
      new Tuple<string, NounType>("Pain", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Passing", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Peasant", NounType.Person),
      new Tuple<string, NounType>("Peasants", NounType.People),
      new Tuple<string, NounType>("Pill", NounType.SingleObject),
      new Tuple<string, NounType>("Phoenix", NounType.Person),
      new Tuple<string, NounType>("Phoenixes", NounType.People),
      new Tuple<string, NounType>("Platinum", NounType.Attribute),
      new Tuple<string, NounType>("Portal", NounType.SingleObject),
      new Tuple<string, NounType>("Prisoner", NounType.Person),
      new Tuple<string, NounType>("Prisoners", NounType.People),
      new Tuple<string, NounType>("Ravenclaw", NounType.Proper),
      new Tuple<string, NounType>("Ring", NounType.SingleObject),
      new Tuple<string, NounType>("Rings", NounType.Object),
      new Tuple<string, NounType>("Robe", NounType.SingleObject),
      new Tuple<string, NounType>("Rumor", NounType.SingleObject),
      new Tuple<string, NounType>("Rumors", NounType.Object),
      new Tuple<string, NounType>("Rune", NounType.SingleObject),
      new Tuple<string, NounType>("Runes", NounType.Object),
      new Tuple<string, NounType>("Savagery", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Sceptor", NounType.SingleObject),
      new Tuple<string, NounType>("Sceptors", NounType.Object),
      new Tuple<string, NounType>("Scroll", NounType.SingleObject),
      new Tuple<string, NounType>("Scrolls", NounType.Object),
      new Tuple<string, NounType>("Secret", NounType.SingleObject),
      new Tuple<string, NounType>("Secrets", NounType.Object),
      new Tuple<string, NounType>("Servant", NounType.Person),
      new Tuple<string, NounType>("Servants", NounType.People),
      new Tuple<string, NounType>("Silver", NounType.Attribute),
      new Tuple<string, NounType>("Skull", NounType.SingleObject),
      new Tuple<string, NounType>("Skulls", NounType.Object),
      new Tuple<string, NounType>("Slytherin", NounType.Proper),
      new Tuple<string, NounType>("Snape", NounType.Proper),
      new Tuple<string, NounType>("Sorcerer", NounType.Person),
      new Tuple<string, NounType>("Sorcerers", NounType.People),
      new Tuple<string, NounType>("Sorrow", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Spear", NounType.SingleObject),
      new Tuple<string, NounType>("Spell", NounType.SingleObject),
      new Tuple<string, NounType>("Spells", NounType.Object),
      new Tuple<string, NounType>("Staff", NounType.SingleObject),
      new Tuple<string, NounType>("Star", NounType.Object),
      new Tuple<string, NounType>("Stars", NounType.SingleObject),
      new Tuple<string, NounType>("Staves", NounType.Object),
      new Tuple<string, NounType>("Statue", NounType.SingleObject),
      new Tuple<string, NounType>("Stone", NounType.Attribute),
      new Tuple<string, NounType>("Story", NounType.SingleObject),
      new Tuple<string, NounType>("Stories", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Student", NounType.Person),
      new Tuple<string, NounType>("Students", NounType.People),
      new Tuple<string, NounType>("Sword", NounType.SingleObject),
      new Tuple<string, NounType>("Swords", NounType.Object),
      new Tuple<string, NounType>("Spy", NounType.Person),
      new Tuple<string, NounType>("Spies", NounType.People),
      new Tuple<string, NounType>("Teacher", NounType.Person),
      new Tuple<string, NounType>("Teachers", NounType.People),
      new Tuple<string, NounType>("the North", NounType.Place),
      new Tuple<string, NounType>("the East", NounType.Place),
      new Tuple<string, NounType>("the West", NounType.Place),
      new Tuple<string, NounType>("the South", NounType.Place),
      new Tuple<string, NounType>("the World", NounType.Place),
      new Tuple<string, NounType>("the Underworld", NounType.Place),
      new Tuple<string, NounType>("the Moon", NounType.Place),
      new Tuple<string, NounType>("the Sun", NounType.Place),
      new Tuple<string, NounType>("Time", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Torch", NounType.SingleObject),
      new Tuple<string, NounType>("Traitor", NounType.Person),
      new Tuple<string, NounType>("Traitors", NounType.People),
      new Tuple<string, NounType>("Troll", NounType.Person),
      new Tuple<string, NounType>("Trolls", NounType.People),
      new Tuple<string, NounType>("Truth", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Valhalla", NounType.Place),
      new Tuple<string, NounType>("Venom", NounType.Attribute),
      new Tuple<string, NounType>("Wand", NounType.SingleObject),
      new Tuple<string, NounType>("Warlock", NounType.Person),
      new Tuple<string, NounType>("Warlocks", NounType.People),
      new Tuple<string, NounType>("Warrior", NounType.Person),
      new Tuple<string, NounType>("Warriors", NounType.People),
      new Tuple<string, NounType>("Wisdom", NounType.DescriptiveThing),
      new Tuple<string, NounType>("Wish", NounType.SingleObject),
      new Tuple<string, NounType>("Wishes", NounType.Object),
      new Tuple<string, NounType>("Witch", NounType.Person),
      new Tuple<string, NounType>("Witches", NounType.People),
      new Tuple<string, NounType>("Wizard", NounType.Person),
      new Tuple<string, NounType>("Wizards", NounType.People),
      new Tuple<string, NounType>("Xanadu", NounType.Place),
    };

    /// <summary>
    /// Return a random title
    /// </summary>
    /// <returns></returns>
    public static string Generate()
    {
      // "Harry Potter and the"
      var str1 = Prefixes[Rng.Next(Prefixes.Count)];

      string str2, str3;

      do
      {
        // "Something [of/'s]"
        var partItem2 = Prepositions[Rng.Next(Prepositions.Count)];
        str2 = partItem2.Item1;
        var filteredNouns = Nouns.Where(e => partItem2.Item2.Has(e.Item2)).ToList();

        // "(Something else)[s]"
        str3 = filteredNouns[Rng.Next(filteredNouns.Count)].Item1;
      } while (str2.Split()[0].Contains(str3) || (str3.Contains(str2.Split()[0])));  //Special case to avoid things like "Sword of Swords"

      return string.Format("{0} {1} {2}", str1, str2, str3);
    }
  }

  internal static class EnumerationExtensions
  {

    public static bool Has<T>(this Enum type, T value)
    {
      try
      {
        return (((int)(object)type & (int)(object)value) == (int)(object)value);
      }
      catch
      {
        return false;
      }
    }

    public static bool Is<T>(this Enum type, T value)
    {
      try
      {
        return (int)(object)type == (int)(object)value;
      }
      catch
      {
        return false;
      }
    }


    public static T Add<T>(this Enum type, T value)
    {
      try
      {
        return (T)(object)(((int)(object)type | (int)(object)value));
      }
      catch (Exception ex)
      {
        throw new ArgumentException(
            string.Format(
                "Could not append value from enumerated type '{0}'.",
                typeof(T).Name
                ), ex);
      }
    }


    public static T Remove<T>(this Enum type, T value)
    {
      try
      {
        return (T)(object)(((int)(object)type & ~(int)(object)value));
      }
      catch (Exception ex)
      {
        throw new ArgumentException(
            string.Format(
                "Could not remove value from enumerated type '{0}'.",
                typeof(T).Name
                ), ex);
      }
    }

  }
}