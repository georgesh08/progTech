package tech.is.service.controllers;

import org.springframework.web.bind.annotation.*;
import tech.is.service.database.CatsDb;
import tech.is.service.database.OwnersDb;
import tech.is.service.entities.Cat;

@RestController
@RequestMapping("cat")
public class CatsController {
    @PostMapping
    public String AddCat(@RequestBody Cat cat){
        if(OwnersDb.IsExisting(cat.Owner)){
            CatsDb.AddCat(cat);
            OwnersDb.AddCatToOwner(cat.Id, cat.Owner);
            return "SUCCESS";
        }
        return "FAILED";
    }

    @GetMapping
    public Cat GetCatById(@RequestParam @PathVariable("id") int id){
        return CatsDb.GetCatById(id);
    }
}
