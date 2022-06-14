package tech.is.service.controllers;

import org.springframework.web.bind.annotation.*;
import tech.is.service.database.OwnersDb;
import tech.is.service.entities.Owner;

@RestController
@RequestMapping("owner")
public class OwnersController {
    @GetMapping
    public Owner GetOwnerById(@RequestParam @PathVariable("id") int id){
        return OwnersDb.GetById(id);
    }

    @PostMapping
    public String AddOwner(@RequestBody Owner owner){
        OwnersDb.AddOwner(owner);
        return "SUCCESS";
    }
}
