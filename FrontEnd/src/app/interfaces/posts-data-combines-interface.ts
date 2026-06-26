import { UserInterface } from "./user-interface"



export interface PostsDataCombinesInterface {
  
    id: string,
    title: string,
    description: string,
    email: string,
    phone: number,
    user: UserInterface,
    createdAt: Date,
    updatedAt: string,
    deletedAt: string
}
