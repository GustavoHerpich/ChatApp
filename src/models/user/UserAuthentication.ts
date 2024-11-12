class UserAuthentication {
  constructor(
    public username: string,
    public password: string,
    public passwordExpiration: string,
    public role: number
  ) {}
}

export default UserAuthentication;
