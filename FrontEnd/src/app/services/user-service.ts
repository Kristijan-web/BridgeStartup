import { Injectable } from '@angular/core';
import { url } from '../consts/consts';
import { UserInterface } from '../interfaces/user-interface';

@Injectable({
  providedIn: 'root',
})
export class UserService {

  // samo mi treba metoda koja ce da dohvati user-a

  async getUser(id: string | undefined): Promise<UserInterface | undefined> {
    try {           
                  
      console.log(`EVO ID-a ` + id);
    const fetchData = await fetch(`${url}/users/${id}`, {
      method: "GET"
    })

    return await fetchData.json();

    }catch(error) {

      if(error instanceof Error) {
        console.log(error.message);
      }
      return undefined;
    }


  }

}
