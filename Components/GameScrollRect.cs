using System;
using UnityEngine;

namespace IG.Module.UI{
    public class GameScrollRect : GameScrollView{
        // [Header("Custom Spacing")]
        // public float right;
        public GameScrollRectSetting Setting{ get => null; set => SetSettings(value); }

        public void SetSettings(GameScrollRectSetting settings){
            this.ItemPrefab = settings.ItemPrefab;
            this.CellSize       = settings.CellSize;
            this.Spacing        = settings.Spacing;
            this.RowCount       = settings.Row;
            this.ColumnCount    = settings.Column;
            this.Top            = settings.Top;
            // this.right = settings.right;
            this.Left   = settings.Left;
            this.Bottom = settings.Bottom;
        }
    }

    [Serializable]
    public class GameScrollRectSetting{
        public GameObject ItemPrefab;

        [Header("Cell Seetings")]
        public Vector2 CellSize;

        public int Row, Column;

        [Header("Spacing Settings")]
        public Vector2 Spacing;

        public float Top, Right, Left, Bottom;
    }
}