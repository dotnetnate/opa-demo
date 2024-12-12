package cfg.data.transforms.mongo

import future.keywords.in

policies := { {"rules": c.rules, "resource": c.resource } | c:= input.incoming[_]  }
