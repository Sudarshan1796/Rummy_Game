using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardManager 
{
    /// <summary>
    /// User turn to draw Card
    /// </summary>
    void Draw();
    /// <summary>
    /// Move Card If required
    /// </summary>
    void Move();
}
