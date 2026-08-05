<?php

namespace App\Auth;

enum LegacyHashVerificationResult
{
    case Verified;
    case VerifiedNeedsRehash;
    case Failed;
    case ForceReset;
}
