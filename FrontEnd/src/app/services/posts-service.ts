import { Injectable, signal } from '@angular/core';
import { PostsInterface } from '../interfaces/posts-interface';
import { url } from '../consts/consts';

@Injectable({
  providedIn: 'root',
})
export class PostsService {




    constructor() {}

  async getAllPosts(): Promise<PostsInterface[]> {

    try {
      const fetchData = await fetch(`${url}/posts`, {
      method: "GET"
    })

    if(!fetchData.ok || fetchData.status >= 400) {
      throw new Error("something went wrong")

    }

   return await fetchData.json();


    }catch(error) {

      if(error instanceof Error) {
      console.log(error.message)
      }

      return []

    }}

  
  async getPost(id: string): Promise<PostsInterface | undefined> {

    try {

    const fetchData = await fetch(`${url}/posts/${id}`, {
      method: "GET"
    })

    if(!fetchData.ok) {
      throw new Error("Something went wrong...")
    }

    return fetchData.json();

    }catch(err) {

      if(err instanceof Error) {
        console.log(err.message);
      }


      return undefined


    }

  }




}
