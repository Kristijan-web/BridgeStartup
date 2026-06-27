import { FetchBackend } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { url } from '../consts/consts';
import { BadgesInterface } from '../interfaces/badges-interface';

@Injectable({
  providedIn: 'root',
})
export class BadgesService {


  async getAllBadges(): Promise<BadgesInterface[]> {
    try {

      const fetchData = await fetch(`${url}/badges`, {
        method: "GET"
      })

      return fetchData.json();

    }catch(err) {
      if (err instanceof Error) {
        console.log(err.message)
      }
      return []
    }
    
  }
}
