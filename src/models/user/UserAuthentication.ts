class UserAuthentication {
  constructor(
    public id: number,
    public username: string,
    public password: string,
    public passwordExpiration: string
  ) {}
}

export default UserAuthentication;
