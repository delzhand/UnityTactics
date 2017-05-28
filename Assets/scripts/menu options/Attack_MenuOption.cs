//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Attack_MenuOption : Command_MenuOption {

//    public override void SelectOption()
//    {
//        base.SelectOption();

//        CombatUnit c = Menu.Attach.GetComponent<CombatUnit>();

//        Material m = Resources.Load("graphics/materials/utility/red_highlight", typeof(Material)) as Material;

//        Tile origin = Menu.Attach.GetComponent<TileOccupier>().GetOccupiedTile();
//        Direction[] checkDirections = new Direction[] { Direction.North, Direction.East, Direction.South, Direction.West };
//        for (int i = 0; i < checkDirections.Length; i++)
//        {
//            Direction d = checkDirections[i]; // direction to check
//            Direction di = checkDirections[(i + 2) % 4]; // direction inverse for edge

//            Tile[] tiles = Engine.TileManager.FindTilesInDirection(origin, d, c.GetAttackHRange());
//            foreach (Tile t in tiles)
//            {
//                if (t != null)
//                {
//                    float difference = origin.Height - t.HeightAtEdge(di);
//                    if (difference <= c.GetAttackVRangeFromAbove() && difference >= -c.GetAttackVRangeFromBelow())
//                    {
//                        t.Highlight(m);
//                        t.CurrentlySelectable = true;
//                    }
//                }
//            }
//        }
//    }

//    public override void Execute(Tile t)
//    {
//        base.Execute(t);
//        Menu.Attach.GetComponent<Facer>().FaceTile(TargetTile);
//        Dash d = Menu.Attach.gameObject.AddComponent<Dash>();
//        d.direction = (int)Menu.Attach.gameObject.GetComponent<Facer>().CurrentDirection()/2;
//        d.delay = .5f;
//        Invoke("Damage", 1f);
//        Invoke("PostExecute", 1.5f);
//    }

//    public void Damage()
//    {
//        TileOccupier tO = TileOccupier.GetTileOccupant(TargetTile);
//        if (tO != null)
//        {
//            FlyingText.CreateFromCharacters("0", tO);
//        }
//    }
//}
