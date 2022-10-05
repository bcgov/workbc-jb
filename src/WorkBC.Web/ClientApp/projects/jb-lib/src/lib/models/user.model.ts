export interface AccountBase {
    email: string;
    username: string;
}

export interface UserBase extends AccountBase {
    firstName: string;
    lastName: string;
}

export interface User extends UserBase {
    id: string;
    token: string;
}
