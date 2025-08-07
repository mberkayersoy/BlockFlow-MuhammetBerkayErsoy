using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementFeature : BaseBlockFeature
{
    [SerializeField] private MovementType _movementType;
    [SerializeField] private List<Sprite> _arrowSprites;

    private SpriteRenderer _spriteRenderer;
    public override void Apply(BlockBehaviour block)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        switch (_movementType)
        {
            default:
            case MovementType.Free:
                _spriteRenderer.enabled = false;
                break;
            case MovementType.Horizontal:
                _spriteRenderer.enabled = true;
                RotateAccordinToBlock();
                break;
            case MovementType.Veritcal:
                _spriteRenderer.enabled = true;
                RotateAccordinToBlock();
                break;
            case MovementType.Static:
                _spriteRenderer.enabled = false;
                break;
        }
    }

    public void RotateAccordinToBlock()
    {
        // To do: Learn shape and fit to shape.
    }
}
