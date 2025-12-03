import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoInstalacionComponent } from './info-instalacion.component';

describe('InfoInstalacionComponent', () => {
  let component: InfoInstalacionComponent;
  let fixture: ComponentFixture<InfoInstalacionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InfoInstalacionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InfoInstalacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
