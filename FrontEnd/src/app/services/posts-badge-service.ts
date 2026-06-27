import { Injectable } from '@angular/core';
import { PostBadgesInterface } from '../interfaces/post-badges-interface';
import { url } from '../consts/consts';

@Injectable({
  providedIn: 'root',
})
export class PostsBadgeService {


  async getPostBadges(id: string): Promise<PostBadgesInterface[] | undefined> {
try {
     const fetchData = await fetch(`${url}/postBadges`, {
       method: "GET"
    })

    return fetchData.json();

}catch(err) {
  if (err instanceof Error) {
    console.log(err.message)
  }
  return undefined;
}
  
  }

}
